using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkTools
{
    //下载系统
    public static int TaskCount => tasks.Count;
    private static readonly object LockMe = new object();
    private static List<Task> tasks = new List<Task>();
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
    public static Dictionary<string, UnityWebRequest> TasksSchedule => tasksSchedule;
    private static readonly Dictionary<string, UnityWebRequest> tasksSchedule = new Dictionary<string, UnityWebRequest>();

    /// <summary>
    /// action会在任务完成时执行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="url"></param>
    /// <param name="callBackAction">会在任务完成时执行</param>
    public static void AddTask(string filePath, string url, Action<string,bool> callBackAction)
    {
        if (DownloaderCount == 0) AddDownloader(1);
        lock (LockMe)
        {
            tasks.Add(new Task(filePath, url, callBackAction));
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
        Action<string,bool> action;
        byte[] data;
        string taskName;
        while (true)
        {
            if (tasks.Count != 0)
            {
                url = tasks[0].value;
                filepath = tasks[0].key;
                action = tasks[0].Action;
                tasks.RemoveAt(0);
                UnityWebRequest request = UnityWebRequest.Get(url);
                taskName = filepath;
                if (!tasksSchedule.TryAdd(taskName, request))
                {
                    int ls=1;
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
                yield return request.SendWebRequest();
                for (int i = 0; i < 3; i++)
                {
                    if (request.isDone && string.IsNullOrEmpty(request.error))
                    {
                        using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
                        {
                            fs.SetLength(0);
                            data = request.downloadHandler.data;
                            fs.Write(data, 0, data.Length);
                        }

                        Debug.Log("下载完成");
                        action?.Invoke(filepath,true);
                        break;
                    }

                    if (i == 2)
                    {
                        Debug.Log("下载失败");
                        Debug.LogWarning(request.error);
                        action?.Invoke(filepath,false);
                        tasksSchedule.Remove(taskName);
                    }
                }
                tasksSchedule[taskName] = null;
                request.Dispose();
            }

            yield return null;
        }
    }

    private class Task
    {
        public string key; // filePath
        public string value; //url
        public Action<string,bool> Action;

        public Task(string k, string v, Action<string,bool> a)
        {
            key = k;
            value = v;
            Action = a;
        }
    }
}