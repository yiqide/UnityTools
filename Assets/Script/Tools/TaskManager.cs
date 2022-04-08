using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Tools
{
    public static class TaskManager
    {
        /// <summary>
        /// 开始并执行一个异步任务
        /// </summary>
        /// <param name="executeTask">执行的任务</param>
        /// <param name="action">当任务完成后会调用 传入ture时任务就是完成的</param>
        public static async void ExecuteTaskAsync(Action executeTask,Action<bool> action=null)
        {
            Task task = new Task(executeTask);
            task.Start();
            await task;
            if (task.IsCompletedSuccessfully)
            {
                action?.Invoke(true);
            }
            else
            {
                Debug.LogError($"任务状态：{task.Status} 信息：{task.Exception}");
                action?.Invoke(false);
            }
        }
        
        /// <summary>
        /// 开始并执行一个异步任务
        /// </summary>
        /// <param name="executeTask">执行的任务</param>
        /// <param name="action">当任务完成后会调用 传入ture时任务就是完成的</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> ExecuteTaskAsync<T>(Func<T> executeTask,Action<bool,T> action=null)
        {
            Task<T> task = new Task<T>(executeTask);
            task.Start();
            await task;
            if (task.IsCompletedSuccessfully)
            {
                action?.Invoke(true,task.Result);
            }
            else
            {
                Debug.LogError($"任务状态：{task.Status} 信息：{task.Exception}");
                action?.Invoke(false,default);
            }
            return task.Result;
        }
    } 
}
