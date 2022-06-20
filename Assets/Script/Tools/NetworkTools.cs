using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Tools
{
    public static class NetworkTools
    {
        #region 开放的信息

        /// <summary>
        /// 当前下载器的数量
        /// </summary>
        public static int DownloaderCount => coroutines.Count;

        #endregion
        
        private static readonly object LockMe = new object();
        
        //顺序等待列表队列 ，分优先级   从小到大排序
        private static SortedDictionary<int, LinkedList<DownLoadTask>> dictionaryTasks = new SortedDictionary<int, LinkedList<DownLoadTask>>();
        //顺序

        //正在下载中的列表
        private static List<DownLoadTask> dowingTasks = new List<DownLoadTask>();
        
        private static List<Coroutine> coroutines = new List<Coroutine>();

        public static void AddTask(DownLoadTask task,bool addFast=false)
        {
            if (!dictionaryTasks.ContainsKey(task.Priority))
            {
                LinkedList<DownLoadTask> linkedList = new LinkedList<DownLoadTask>();
                linkedList.AddFirst(task);
                dictionaryTasks.Add(task.Priority,linkedList);
            }
            else
            {
                //不在添加重复的任务
                if (dowingTasks.Contains(task))
                {
                    return;
                }
                if (dictionaryTasks[task.Priority].Contains(task))
                {
                    return;
                }
                if (addFast)
                    dictionaryTasks[task.Priority].AddFirst(task);
                else
                    dictionaryTasks[task.Priority].AddLast(task);
            }
        }

        public static void AddTasks(DownLoadTasks downLoadTasks)
        {
            foreach (var item in downLoadTasks.Tasks)
            {
                AddTask(item);
            }
        }
        //从下载列表中移除
        public static bool TryRemoveTask(DownLoadTask task)
        {
            if (dictionaryTasks.ContainsKey(task.Priority))
            {
                if (dictionaryTasks[task.Priority].Contains(task))
                {
                    dictionaryTasks[task.Priority].Remove(task);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加下载器，在任务多的时候会提升下载速度
        /// </summary>
        /// <param name="count">要添加下载器的数量</param>
        public static void AddDownloader(int count)
        {
            lock (LockMe)
            {
                for (int i = 0; i < count; i++)
                {
                    coroutines.Add(CoroutineTools.Instance.StartCoroutine(DownLoad()));
                }
            }
        }

        //获取一个任务 并从等待列表中移除
        private static DownLoadTask GetTask()
        {
            foreach (var item in dictionaryTasks)
            {
                if (item.Value.Count!=0)
                {
                    var task = item.Value.First.Value;
                    item.Value.RemoveFirst();
                    return task;
                }
            }
            return null;
        }

        //下载器
        private static IEnumerator DownLoad()
        {
            byte[] data;
            DownLoadTask task=null;
            while (true)
            {
                task = GetTask();
                for (int i = 0; i < task.RestartCount; i++)
                {
                    if (task != null)
                    {
                        dowingTasks.Add(task);
                        task.SetStatus(DownLoadTaskStatus.DownLoading);
                        UnityWebRequest request = UnityWebRequest.Get(task.Url);
                        if (task.Timeout != 0) request.timeout = task.Timeout;
                        task.UnityWebRequest = request;
                        string savePath = Path.GetDirectoryName(task.FilePath);
                        if (!string.IsNullOrEmpty(savePath) && !Directory.Exists(savePath))
                            Directory.CreateDirectory(savePath);
                        yield return request.SendWebRequest();
                        if (request.isDone && string.IsNullOrEmpty(request.error))
                        {
                            using (FileStream fs = new FileStream(task.FilePath, FileMode.OpenOrCreate))
                            {
                                fs.SetLength(0);
                                data = request.downloadHandler.data;
                                fs.Write(data, 0, data.Length);
                            }
                            //Debug.Log("下载完成");
                            task.SetStatus(DownLoadTaskStatus.Complete);
                            task.succeedCallBack?.Invoke(task);
                            dowingTasks.Remove(task);
                            request.Dispose();
                            task.UnityWebRequest = null;
                            goto jump;//跳出循环
                        }
                        else
                        {
                            //Debug.Log("下载失败");
                            try
                            {
                                File.Delete(task.FilePath);
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning(e);
                            }

                            task.SetStatus(DownLoadTaskStatus.Fail);
                            dowingTasks.Remove(task);
                            //最后一次失败就调用失败的方法
                            if (i==task.RestartCount-1) task.failCallBack?.Invoke(task, request.error);
                            request.Dispose();
                            task.UnityWebRequest = null;
                        }
                    }
                }
                jump:
                yield return null;
            }
        }

        //单个任务
        public class DownLoadTask
        {
            public string FilePath => filePath;
            public string Url => url;
            /// <summary>
            /// 成功后和失败后都是null
            /// </summary>
            public UnityWebRequest UnityWebRequest 
            {
                get { return unityWebRequest; }
                set { unityWebRequest = value; }
            }
            /// <summary>
            /// 任务标识符,目前没什么用
            /// </summary>
            public string TaskID => taskID;
            public DownLoadTaskStatus Status => status;
            public event Action<DownLoadTask> SucceedCallBack
            {
                add { succeedCallBack += value;}
                remove { succeedCallBack -= value;}
            }
            public event Action<DownLoadTask,string> FailCallBack
            {
                add { failCallBack += value;}
                remove { failCallBack -= value; }
            }
            public Action<DownLoadTask> succeedCallBack;
            public Action<DownLoadTask,string> failCallBack;
            public int Timeout => timeout;
            public int Priority=>priority;
            public int RestartCount=>restartCount;//重试次数
            private int priority;//优先级 0 是最高优先级
            private int restartCount;//重试次数
            private int timeout;
            private DownLoadTaskStatus status;
            private string filePath;
            private string url;
            private UnityWebRequest unityWebRequest = null;
            private string taskID;
            public void SetStatus(DownLoadTaskStatus status)
            {
                this.status = status;
            }
            public DownLoadTask(string filePath, string url,int priority,int reStartCount=2,int timeout=0,string taskID = null)
            {
                this.filePath = filePath;
                this.url = url;
                this.taskID = taskID;
                this.timeout = timeout;
                this.priority = priority;
                restartCount = reStartCount;
                status = DownLoadTaskStatus.None;
            }
        }

        //多个任务组成一个任务组
        public class DownLoadTasks
        {
            public DownLoadTask[] Tasks => tasks;
            public int CompleteCount => succeedCount;
            public float DownLoadSchedule => ((float) CompleteCount) / tasks.Length;
            /// <summary>
            /// 所有任务成功时的回调
            /// </summary>
            public event Action<DownLoadTasks> SucceedCallBack
            {
                add { succeedCallBack += value;}
                remove { succeedCallBack -= value;}
            }
            /// <summary>
            /// 有任务失败时，会移除在等待列表的任务
            /// 正在下载的任务不受影响，直到任务返回结果
            /// 之后会收集报错信息 并调用该方法
            /// </summary>
            public event Action<DownLoadTasks,string[]> FailCallBack
            {
                add { failCallBack += value;}
                remove { failCallBack -= value; }
            }
            private Action<DownLoadTasks> succeedCallBack;
            private Action<DownLoadTasks, string[]> failCallBack;
            
            private DownLoadTask[] tasks;

            public DownLoadTasks(DownLoadTask[] tasks)
            {
                this.tasks = tasks;
                foreach (var item in tasks)
                {
                    item.FailCallBack += FailBack;
                    item.SucceedCallBack += SucceedBack;
                }
            }

            private int succeedCount=0;
            private int resultCount = 0;
            private void SucceedBack(DownLoadTask task)
            {
                resultCount++;
                if (succeedCount==Tasks.Length)
                {
                    succeedCallBack?.Invoke(this);
                }
                if (failFlag&&resultCount==Tasks.Length)
                {
                    failCallBack.Invoke(this,msgs.ToArray());
                }
            }

            private bool failFlag = false;
            private List<string> msgs=new List<string>();
            private void FailBack(DownLoadTask task,string msg)
            {
                resultCount++;
                msgs.Add(msg);
                if (failFlag)
                {
                    int count=0;
                    foreach (var item in Tasks)
                    {
                        if ( NetworkTools.TryRemoveTask(item))
                        {
                            count++;
                        }
                    }
                    resultCount += count;
                    failFlag = true;
                }

                if (resultCount==Tasks.Length)
                {
                    failCallBack.Invoke(this,msgs.ToArray());
                }
            }
            private bool callBackFalg = false;
        }

        public enum DownLoadTaskStatus
        {
            None,
            DownLoading,
            Complete,
            Fail
        }
    }
}