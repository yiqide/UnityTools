using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XFABManager
{
    public class ReadyResRequest : CustomAsyncOperation<ReadyResRequest>
    {

        /// <summary>
        /// 当前正在执行什么操作 比如:更新下载等等
        /// </summary>
        public UpdateType updateType { get; private set; }

        /// <summary>
        /// 当前正在 准备的模块的名称
        /// </summary>
        public string currentProjectName { get; private set; }

        /// <summary>
        /// 下载速度 单位:字节
        /// </summary>
        public long CurrentSpeed { get; protected set; }

        /// <summary>
        /// 格式化之后的下载速度
        /// </summary>
        public string CurrentSpeedFormatStr { get; protected set; } = string.Empty;


        internal IEnumerator ReadyRes(string projectName)
        {
            if (string.IsNullOrEmpty(projectName)) {
                yield return new WaitForEndOfFrame();
                Completed(string.Format("项目名不能为空!"));
                yield break;
            }
            if (!AssetBundleManager.isInited)
            {
                // 初始化
                yield return AssetBundleManager.InitializeAsync();
            }
            // 检测更新
            CheckResUpdatesRequest checkReq = AssetBundleManager.CheckResUpdates(projectName);
            yield return checkReq;
            if (!string.IsNullOrEmpty(checkReq.error))
            {
                error = string.Format("准备资源失败,检测更新出错:{0}", checkReq.error);
                Completed();
                yield break;
            }
            yield return CoroutineStarter.Start(ReadyRes(checkReq.results));
            Completed();
        }


        internal IEnumerator ReadyRes(CheckUpdateResult[] results)
        {

            if (!AssetBundleManager.isInited)
            {
                // 初始化
                yield return AssetBundleManager.InitializeAsync();
            }

            // 具体操作
            for (int i = 0; i < results.Length; i++)
            {
                updateType = results[i].updateType;
                currentProjectName = results[i].projectName;

                switch (results[i].updateType)
                {
                    case UpdateType.Update:
                    case UpdateType.Download:
                    case UpdateType.DownloadZip:

                        // 更新或者下载资源
                        UpdateOrDownloadResRequest downloadReq = AssetBundleManager.UpdateOrDownloadRes(results[i]);

                        while (!downloadReq.isDone)
                        {
                            yield return null;
                            progress = downloadReq.progress / results.Length * (i + 1);
                            CurrentSpeed = downloadReq.CurrentSpeed;
                            CurrentSpeedFormatStr = downloadReq.CurrentSpeedFormatStr;
                        }

                        if (!string.IsNullOrEmpty(downloadReq.error))
                        {
                            error = string.Format("准备资源失败,下载出错:{0}", downloadReq.error);
                            Completed();
                            yield break;
                        }

                        break;
                    case UpdateType.ExtractLocal:
                    //case UpdateType.ExtractLocalZip:

                        // 释放资源
                        ExtractResRequest extractReq = AssetBundleManager.ExtractRes(results[i]);
                        while (!extractReq.isDone)
                        {
                            yield return null;
                            progress = extractReq.progress / results.Length * (i + 1) * 0.5f;
                        }

                        if (!string.IsNullOrEmpty(extractReq.error))
                        {
                            error = string.Format("准备资源失败,释放资源出错:{0}", extractReq.error);
                            Completed();
                            yield break;
                        }

                        // 再次调用 准备资源方法 检测是否需要更新资源
                        ReadyResRequest readyUpdate = AssetBundleManager.ReadyRes(results[i].projectName);
                        // 释放完成之后 还需要再检测是否需要更新 
                        while (!readyUpdate.isDone)
                        {
                            yield return null;
                            progress = readyUpdate.progress / results.Length * (i + 1) * 0.5f + 1 / results.Length * (i + 1) * 0.5f;
                        }

                        if ( !string.IsNullOrEmpty(readyUpdate.error) ) {
                            Completed(readyUpdate.error);
                        }

                        
                        break;
                    case UpdateType.Error:
                        // 出错
                        error = string.Format("准备资源失败,{0}", results[i].message);
                        Completed();
                        yield break;
                }

                //yield return null;
                //progress = 0;

            }

        }

    }

}

