using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XFABManager
{
    public class UpdateOrDownloadResRequest : CustomAsyncOperation<UpdateOrDownloadResRequest>
    {

        /// <summary>
        /// 下载速度 单位:字节
        /// </summary>
        public long CurrentSpeed { get; protected set; }

        /// <summary>
        /// 格式化之后的下载速度
        /// </summary>
        public string CurrentSpeedFormatStr { get; protected set; } = string.Empty;

        internal IEnumerator UpdateOrDownloadRes(string projectName) {
            if (!AssetBundleManager.isInited)
            {
                // 初始化
                yield return AssetBundleManager.InitializeAsync();
            }

            CheckResUpdateRequest requestUpdate = AssetBundleManager.CheckResUpdate(projectName);
            yield return requestUpdate;

            if ( !string.IsNullOrEmpty( requestUpdate.error ) ) {
                Completed(requestUpdate.error);
                yield break;
            }
            yield return CoroutineStarter.Start(UpdateOrDownloadRes(requestUpdate.result));
        }

        /// <summary>
        /// 更新或下载资源
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal IEnumerator UpdateOrDownloadRes(CheckUpdateResult result) {

            if (!AssetBundleManager.isInited)
            {
                // 初始化
                yield return AssetBundleManager.InitializeAsync();
            }

            // 判断是不是下载 压缩包
            if (result.updateType == UpdateType.DownloadZip)
            {
                string zipUrl = XFABTools.ServerPath(AssetBundleManager.GetProfile(result.projectName).url, result.projectName, result.version, string.Format("{0}.zip", result.projectName) );
                string localfile = XFABTools.LocalResPath(result.projectName, string.Format("{0}{1}", result.projectName, ".zip"));

                DownloadFileRequest requestZip = DownloadFileRequest.Download(zipUrl, localfile);
                //DownloadFileRequest requestZip = new DownloadFileRequest();
                //CoroutineStarter.Start(requestZip.Download(zipUrl, localfile));
                while ( !requestZip.isDone ) {
                    yield return null;
                    progress = requestZip.progress;
                    CurrentSpeed = requestZip.Speed;
                    //CurrentSpeedFormatStr = requestZip.CurrentSpeedFormatStr;
                }
                if (!string.IsNullOrEmpty(requestZip.error)) {
                    Completed(requestZip.error);
                    yield break; 
                }

                try
                {
                    // 进行解压
                    ZipTools.UnZipFile(localfile, XFABTools.DataPath(result.projectName));
                }
                catch (System.Exception e)
                {
                    Debug.LogErrorFormat("解压出错:{0} error:{1}", localfile, e.ToString());
                }
                // 解压完成之后 删除压缩包
                File.Delete(localfile);
            }
            else if(result.updateType == UpdateType.Update || result.updateType == UpdateType.Download)
            {
                // 更新下载
                string fileUrl = null;
                string localFile = null;

                //Dictionary<string, string> files = new Dictionary<string, string>();
                List<DownloadObjectInfo> files = new List<DownloadObjectInfo>();
                

                for (int i = 0; i < result.need_update_bundles.Length; i++)
                {
                    fileUrl = XFABTools.ServerPath(AssetBundleManager.GetProfile(result.projectName).url, result.projectName, result.version, result.need_update_bundles[i].bundleName);// 这个文件名是包含后缀的
                    localFile = XFABTools.LocalResPath(result.projectName, result.need_update_bundles[i].bundleName);

                    //requestFiles.Add(fileUrl, localFile);

                    files.Add( new DownloadObjectInfo( fileUrl, localFile, result.need_update_bundles[i].bundleSize));
                }
                DownloadFilesRequest requestFiles = DownloadFilesRequest.DownloadFiles(files);
                //CoroutineStarter.Start(requestFiles.Download());
                while (  !requestFiles.isDone)
                {
                    yield return null;
                    progress = requestFiles.progress;
                    CurrentSpeed = requestFiles.Speed;
                    //CurrentSpeedFormatStr = requestFiles.CurrentSpeedFormatStr;
                }

                if ( !string.IsNullOrEmpty(requestFiles.error) )
                {
                    Completed(requestFiles.error/*, requestFiles.file_url*/);
                    yield break;
                }

                // 验证下载的资源是否正确 
                CheckResUpdateRequest requestUpdate = AssetBundleManager.CheckResUpdate(result.projectName);
                yield return requestUpdate;

                // 验证出错
                if (!string.IsNullOrEmpty(requestUpdate.error))
                {
                    Completed(requestUpdate.error);
                    yield break;
                }

                if ( requestUpdate.result.updateType != UpdateType.DontNeedUpdate ) {
                    // 说明资源不匹配 需要重新下载
                    yield return CoroutineStarter.Start(UpdateOrDownloadRes(requestUpdate.result));
                }

            }

            // 保存 Version
            XFABTools.SaveVersion(result.projectName, result.version);

            // 下载完成
            Completed();

        }

    }

}

