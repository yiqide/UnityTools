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
        
        /// <summary>
        /// 当前正在下载的任务
        /// 下载失败的任务将会从里面移除,下载成功的也会移除
        /// string-任务名称  UnityWebRequest-下载对象
        /// </summary>
        public static Dictionary<string, UnityWebRequest> TasksSchedule => tasksSchedule;
        
        #endregion
        
        private static readonly object LockMe = new object();
        //等待下载的列表
        private static List<Task> tasks = new List<Task>();
        //下载中的列表
        private static List<Task> dowingTasks = new List<Task>();
        
        private static List<Coroutine> coroutines = new List<Coroutine>();
        
        private static readonly Dictionary<string, UnityWebRequest> tasksSchedule = new Dictionary<string, UnityWebRequest>();

        /// <summary>
        /// action会在任务完成时执行
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="url"></param>
        /// <param name="callBackAction">会在任务完成时执行</param>
        public static void AddTask(string filePath, string url, Action<bool> callBackAction = null)
        {
            if (DownloaderCount == 0) AddDownloader(1);
            AddTask(new Task(filePath, url, callBackAction));
        }

        private static void AddTask(Task task)
        {
            if (DownloaderCount == 0) AddDownloader(1);
            lock (LockMe)
            {
                foreach (var item in dowingTasks)
                {
                    if ( item.key==task.key)
                    {
                        return;
                    }
                }

                foreach (var item in tasks)
                {
                    if ( item.key==task.key)
                    {
                        return;
                    }
                }
                tasks.Add(task);
            }
        }

        /// <summary>
        /// action会在任务组完成时执行
        /// </summary>
        /// <param name="filePathAndUrl">key-filePath  value-url</param>
        /// <param name="callBackAction"></param>
        public static void AddTasks(Dictionary<string, string> filePathAndUrl, Action<bool> callBackAction = null)
        {
            if (DownloaderCount == 0) AddDownloader(1);
            var count = filePathAndUrl.Count;
            int taskSucceedCount = 0;
            bool taskFailed = false;
            foreach (var item in filePathAndUrl)
            {
                var task = new Task(item.Key, item.Value, (b) =>
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
                AddTask(task);
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
            string url;
            string filepath;
            Action<bool> action;
            byte[] data;
            string taskName;
            while (true)
            {
                if (tasks.Count != 0)
                {
                    Task task = tasks[0];
                    url = task.value;
                    filepath = task.key;
                    action = task.Action;
                    dowingTasks.Add(task);
                    tasks.Remove(task);
                    UnityWebRequest request = UnityWebRequest.Get(url);
                    taskName = filepath;
                    if (!tasksSchedule.TryAdd(taskName, request))
                    {
                        int ls = 1;
                        while (true)
                        {
                            taskName = filepath + ls;
                            if (!tasksSchedule.TryAdd(taskName, request))
                            {
                                ls++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    string savePath = Path.GetDirectoryName(filepath);
                    if (!string.IsNullOrEmpty(savePath)&&!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
                    yield return request.SendWebRequest();
                    if (request.isDone && string.IsNullOrEmpty(request.error))
                    {
                        using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
                        {
                            fs.SetLength(0);
                            data = request.downloadHandler.data;
                            fs.Write(data, 0, data.Length);
                        }
                        Debug.Log("下载完成");
                        dowingTasks.Remove(task);
                        action?.Invoke(true);
                    }
                    else
                    {
                        Debug.Log("下载失败");
                        Debug.LogWarning(request.error);
                        try
                        {
                            File.Delete(filepath);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e);
                        }

                        dowingTasks.Remove(task);
                        tasksSchedule.Remove(taskName);
                        action?.Invoke(false);
                    }
                    request.Dispose();
                    tasksSchedule.Remove(taskName);
                }
                yield return null;
            }
        }

        private class Task
        {
            public string key; // filePath
            public string value; //url
            public Action<bool> Action;

            public Task(string k, string v, Action<bool> a)
            {
                key = k;
                value = v;
                Action = a;
            }
        }
    }
}