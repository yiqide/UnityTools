using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkTools
{
    //下载系统
    private static int taskCount = 0;
    public static int TaskCount => taskCount;
    private static readonly object LockMe = new object();
    private static List<Task> _tasks = new List<Task>();
    /// <summary>
    /// 当前下载器的数量
    /// </summary>
    public static int DownloaderCount => coroutines.Count;
    private static List<Coroutine> coroutines = new List<Coroutine>();
    
    /// <summary>
    /// 下载成功的任务 UnityWebRequest 是null
    /// 下载失败的任务将会从里面移除
    /// string-任务名称  UnityWebRequest-下载对象
    /// </summary>
    public static Dictionary<string, UnityWebRequest> TasksSchedule => _tasksSchedule;
    private static readonly Dictionary<string, UnityWebRequest> _tasksSchedule = new Dictionary<string, UnityWebRequest>();

    /// <summary>
    /// action会在任务完成时执行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="url"></param>
    /// <param name="callBackAction">会在任务完成时执行</param>
    public static void AddTask(string filePath, string url, Action<bool> callBackAction)
    {
        if (DownloaderCount == 0) AddDownloader(1);
        lock (LockMe)
        {
            _tasks.Add(new Task(filePath, url, callBackAction));
            taskCount++;
        }
    }

    /// <summary>
    /// 添加下载器，在任务多的时候可以进行并行下载
    /// </summary>
    /// <param name="count">下载器的数量</param>
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
            if (_tasks.Count != 0)
            {
                url = _tasks[0].value;
                filepath = _tasks[0].key;
                action = _tasks[0].Action;
                _tasks.RemoveAt(0);
                UnityWebRequest request = UnityWebRequest.Get(url);
                taskName = filepath;
                if (!_tasksSchedule.TryAdd(taskName, request))
                {
                    int ls=1;
                    while (true)
                    {
                        taskName = filepath + ls;
                        if (!_tasksSchedule.TryAdd(taskName, request))
                        {
                            ls++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                yield return request.SendWebRequest();
                for (int i = 0; i < 3; i++)
                {
                    if (request.isDone && string.IsNullOrEmpty(request.error))
                    {
                        using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
                        {
                            data = request.downloadHandler.data;
                            fs.Write(data, 0, data.Length);
                        }

                        Debug.Log("下载完成");
                        taskCount--;
                        action?.Invoke(true);
                        break;
                    }

                    if (i == 2)
                    {
                        Debug.Log("下载失败");
                        Debug.Log(request.error);
                        action?.Invoke(false);
                        taskCount--;
                        _tasksSchedule.Remove(taskName);
                    }
                }
                _tasksSchedule[taskName] = null;
                request.Dispose();
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