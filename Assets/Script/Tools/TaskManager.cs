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
        /// <param name="overtime">任务超时时间，默认为0 -任务没有不会存在超时，但也可能永远不会返回  单位为毫秒</param>
        /// <param name="action">当任务完成后会调用 传入ture时任务就是完成的</param>
        public static async void ExecuteTaskAsync(Action executeTask,int overtime=0,Action<bool> action=null)
        {
            Task task = Task.Factory.StartNew(executeTask);
            if (overtime==0)
            {
                await task;
            }
            else
            {    
                await Task.Delay(overtime);
            }
            if (task.IsCompletedSuccessfully)
            {
                action?.Invoke(true);
                task.Dispose();
            }
            else
            {
                Debug.LogError($"任务状态：{task.Status}\n信息：{task.Exception}");
                action?.Invoke(false);
                if (task.IsFaulted || task.IsCanceled)
                {
                    task.Dispose();
                }
            }
        }
        
        /// <summary>
        /// 开始并执行一个异步任务
        /// </summary>
        /// <param name="executeTask">执行的任务</param>
        /// <param name="overtime">任务超时时间，默认为0 -任务没有不会存在超时，但也可能永远不会返回   单位为毫秒</param>
        /// <param name="action">当任务完成后会调用 传入ture时任务就是完成的</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> ExecuteTaskAsync<T>(Func<T> executeTask,int overtime,Action<bool,T> action=null)
        {
            Task<T> task =  Task<T>.Factory.StartNew(executeTask);
            if (overtime==0)
            {
                await task;
            }
            else
            {
                await Task.Delay(overtime);
            }
            if (task.IsCompletedSuccessfully)
            {
                action?.Invoke(true,task.Result);
                task.Dispose();
            }
            else
            {
                Debug.LogError($"任务状态：{task.Status}\n信息：{task.Exception}");
                action?.Invoke(false,default);
                if (task.IsFaulted || task.IsCanceled)
                {
                    task.Dispose();
                }
            }
            return task.Result;
        }
    } 
}
