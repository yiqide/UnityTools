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
        //下载系统
        public static int WaitTaskCount => tasks.Count;

        /// <summary>
        /// 当前下载器的数量
        /// </summary>
        public static int DownloaderCount => coroutines.Count;

        #endregion
        
        private static readonly object LockMe = new object();
        //等待下载的列表
        private static LinkedList<DownLoadTask> tasks = new LinkedList<DownLoadTask>();
        //下载中的列表
        private static List<DownLoadTask> dowingTasks = new List<DownLoadTask>();
        
        private static List<Coroutine> coroutines = new List<Coroutine>();
        
        /// <summary>
        /// action会在任务完成时执行
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="url"></param>
        /// <param name="callBackAction">会在任务完成时执行</param>
        public static void AddTask(string filePath, string url, Action<bool> callBackAction = null)
        {
            if (DownloaderCount == 0) AddDownloader(1);
            AddTask(new DownLoadTask(filePath, url, callBackAction));
        }

        public static void AddTask(DownLoadTask task,bool addFast=false)
        {
            if (DownloaderCount == 0) AddDownloader(1);
            lock (LockMe)
            {
                foreach (var item in dowingTasks)
                {
                    if ( item.FilePath==task.FilePath)
                    {
                        return;
                    }
                }

                foreach (var item in tasks)
                {
                    if ( item.FilePath==task.FilePath)
                    {
                        return;
                    }
                }

                if (addFast) 
                    tasks.AddFirst(task);
                else
                    tasks.AddLast(task);

            }
        }

        /// <summary>
        /// action会在任务组完成时执行
        /// </summary>
        /// <param name="filePathAndUrl">key-filePath  value-url</param>
        /// <param name="callBackAction"></param>
        public static void AddTasks(Dictionary<string, string> filePathAndUrl,bool addFast=false, Action<bool> callBackAction = null)
        {
            if (DownloaderCount == 0) AddDownloader(1);
            var count = filePathAndUrl.Count;
            int taskSucceedCount = 0;
            bool taskFailed = false;
            foreach (var item in filePathAndUrl)
            {
                var task = new DownLoadTask(item.Key, item.Value, (b) =>
                {
                    if (b)
                    {
                        taskSucceedCount++;
                        if (taskSucceedCount == count)
                        {
                            callBackAction?.Invoke(true);
                        }
                    }
                    else
                    {
                        if (!taskFailed)
                        {
                            callBackAction?.Invoke(false);
                            taskFailed = true;
                        }
                    }
                });
                AddTask(task,addFast);
            }
        }
        
        public static void AddTasks(DownLoadTasks downLoadTasks,bool addFast=false)
        {
            if (DownloaderCount == 0) AddDownloader(1);
            foreach (var item in downLoadTasks.Tasks)
            {
                AddTask( item,addFast);
            }
        }
        
        /// <summary>
        /// 添加下载器，在任务多的时候会提升下载速度
        /// </summary>
        /// <param name="count">要添加下载器的数量</param>
        public static void AddDownloader(int count)
        {
            for (int i = 0; i < count; i++)
            {
                coroutines.Add(CoroutineTools.Instance.StartCoroutine(DownLoad()));
            }
        }

        private static IEnumerator DownLoad()
        {
            byte[] data;
            while (true)
            {
                if (tasks.Count != 0)
                {
                    DownLoadTask task = tasks.First.Value;
                    dowingTasks.Add(task);
                    tasks.Remove(task);
                    UnityWebRequest request = UnityWebRequest.Get(task.Url);
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

                        Debug.Log("下载完成");
                        dowingTasks.Remove(task);
                        task.Action?.Invoke(true);
                    }
                    else
                    {
                        Debug.Log("下载失败");
                        Debug.LogWarning(request.error);
                        try
                        {
                            File.Delete(task.FilePath);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e);
                        }

                        dowingTasks.Remove(task);

                        task.Action?.Invoke(false);
                    }

                    request.Dispose();
                    task.UnityWebRequest = null;
                }

                yield return null;
            }
        }

        public class DownLoadTask
        {
            public string FilePath => filePath;
            public string Url => url;

            public UnityWebRequest UnityWebRequest //成功后和失败后都是null
            {
                get { return unityWebRequest; }
                set { unityWebRequest = value; }
            }

            public string TaskName => taskName;
            public DownLoadTaskStatus Status => status;
            public Action<bool> Action => action;

            private DownLoadTaskStatus status;
            private string filePath;
            private string url;
            private Action<bool> action;
            private UnityWebRequest unityWebRequest = null;
            private string taskName;

            public DownLoadTask(string filePath, string url, Action<bool> action = null, string taskName = null)
            {
                this.filePath = filePath;
                this.url = url;
                this.action = action;
                this.taskName = taskName;
                status = DownLoadTaskStatus.None;
            }
        }

        public class DownLoadTasks
        {
            public List<DownLoadTask> Tasks => tasks;

            public int CompleteCount()
            {
                int result = 0;
                foreach (var item in tasks)
                {
                    if (item.Status == DownLoadTaskStatus.Complete)
                    {
                        result++;
                    }
                }

                return result;
            }

            public float DownLoadSchedule => ((float) CompleteCount()) / tasks.Count;

            private Action<bool> Action;
            private List<DownLoadTask> tasks;

            public DownLoadTasks(Dictionary<string, string> filePathAndUrl, Action<bool> action)
            {
                var tasks = new List<DownLoadTask>();
                Action = action;
                foreach (var item in filePathAndUrl)
                {
                    tasks.Add(new DownLoadTask(item.Key, item.Value, (b) =>
                    {
                        if (b)
                        {
                            CallBack();
                        }
                        else
                        {
                            if (!callBackFalg)
                            {
                                callBackFalg = true;
                                Action.Invoke(false);
                            }
                        }
                    }));
                }

                this.tasks = tasks;
            }

            private bool callBackFalg = false;

            private void CallBack()
            {
                if (CompleteCount() == Tasks.Count)
                {
                    callBackFalg = true;
                    Action.Invoke(true);
                }
            }
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