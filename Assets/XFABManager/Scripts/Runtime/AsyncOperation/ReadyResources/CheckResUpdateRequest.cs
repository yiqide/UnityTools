using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace XFABManager
{

    /// <summary>
    /// 检测资源 不包含依赖项目
    /// </summary>
    public class CheckResUpdateRequest : CustomAsyncOperation<CheckResUpdateRequest>
    {
        //private CheckUpdateResult _result;
        public CheckUpdateResult result { get; private set; }



        internal IEnumerator CheckResUpdate(string projectName)
        {

            if (string.IsNullOrEmpty(projectName))
            {
                yield return new WaitForEndOfFrame();
                Completed(string.Format("项目名不能为空!"));
                yield break;
            }

            if (!AssetBundleManager.isInited)
            {
                // 初始化
                yield return AssetBundleManager.InitializeAsync();
            }

            // 构建检测结果
            result = new CheckUpdateResult(projectName);
            // 根据更新模式 检测本地资源 判断本地是否 有资源 是否有内置资源 给出更新方案
            yield return CoroutineStarter.Start(CheckLocalRes(projectName));
            // 如果检测结果 需要更新或下载 再进行网络检测
            if (result.updateType == UpdateType.Update || result.updateType == UpdateType.Download)
            {
                // 获取项目版本
                GetProjectVersionRequest requestVersion = AssetBundleManager.GetProjectVersion(projectName);
                yield return requestVersion;

                if (!string.IsNullOrEmpty(requestVersion.error))
                {
                    // 检测出错
                    result.updateType = UpdateType.Error;
                    Completed(string.Format("获取版本出错:{0}", requestVersion.error));
                    yield break;
                }
                result.version = requestVersion.version;
                // 检测是否有压缩文件
                if (result.updateType == UpdateType.Download)
                {
                    if (CheckServerZip(projectName, requestVersion.version))
                    {
                        Completed();
                        yield break;
                    }
                }
                // 获取服务器的文件列表信息
                ProjectBuildInfo projectBuildInfo = null;

                GetFileFromServerRequest requestFile = AssetBundleManager.GetFileFromServer(projectName, requestVersion.version, XFABConst.project_build_info);
                yield return requestFile;

                if (!string.IsNullOrEmpty(requestFile.error))
                {
                    result.updateType = UpdateType.Error;
                    error = string.Format("获取项目{0}文件列表出错:{1}", projectName, requestFile.error);
                    result.message = error;
                    isCompleted = true;
                    yield break;
                }
                else
                {
                    projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(requestFile.text);
                }

                // 判断本地版本号 与 服务端版本号 是否相同
                if (!XFABTools.LocalResVersion(projectName).Equals(requestVersion.version))
                {
                    // 如果版本号 不相同 获取到服务端文件列表 与本地进行比较 找出需要更新的文件
                    yield return CoroutineStarter.Start(CompareWithLocal(projectName, projectBuildInfo.bundleInfos));
                    // 获取更新内容
                    result.message = projectBuildInfo.update_message;
                }
                else {
                    result.updateType = UpdateType.DontNeedUpdate;
                }

            }

            isCompleted = true;
        }

        /// <summary>
        /// 检测服务端是否有zip文件
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool CheckServerZip(string projectName, string version)
        {
            if (result.updateType == UpdateType.Download)
            {
                try
                {
                    string zipPath = XFABTools.ServerPath(AssetBundleManager.GetProfile(projectName).url, projectName, version, string.Format("{0}.zip", projectName));
                    //创建根据网络地址的请求对象
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(zipPath));
                    httpWebRequest.Method = "HEAD";
                    httpWebRequest.Timeout = 1000; //返回响应状态是否是成功比较的布尔值
                    HttpWebResponse webResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        result.updateType = UpdateType.DownloadZip;
                        result.updateSize = webResponse.ContentLength;
                        return true;
                    }
                }
                catch { }

            }

            return false;
        }


        /// <summary>
        /// 与本地的文件比较 判断是否需要更新
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="files"></param>
        private IEnumerator CompareWithLocal(string projectName, BundleInfo[] bundleInfos)
        {

            List<BundleInfo> need_update_files = new List<BundleInfo>();

            for (int i = 0; i < bundleInfos.Length; i++)
            {

                bool isNeedUpdate = false;
                // 判断本地是否存在
                string localFile = XFABTools.LocalResPath(projectName, bundleInfos[i].bundleName);
                if (File.Exists(localFile))
                {

                    //Task<string> md5 = XFABTools.md5fileasync(localFile);
                    //while (!md5.IsCompleted)
                    //{
                    //    yield return null;
                    //}

                    FileMD5Request fileMD5 = XFABTools.CaculateFileMD5(localFile);
                    yield return fileMD5;

                    if (!string.IsNullOrEmpty(fileMD5.error)) {
                        Completed( string.Format("计算md5失败:{0} error:{1}",localFile, fileMD5.error));
                        yield break;
                    }

                    // 判断md5值是否相同
                    isNeedUpdate = !fileMD5.md5.Equals(bundleInfos[i].md5);
                }
                else
                {
                    // 本地没有 需要更新
                    isNeedUpdate = true;
                }

                if (isNeedUpdate)
                {
                    result.updateSize += bundleInfos[i].bundleSize;
                    need_update_files.Add(bundleInfos[i]);
                }
            }

            // 判断是否需要更新
            if (result.updateSize == 0 || need_update_files.Count == 0)
            {
                result.updateType = UpdateType.DontNeedUpdate;
                // 更新一下本地版本号
                XFABTools.SaveVersion(result.projectName, result.version);
            }
            else
            {
                // 需要更新 同时需要更新 project_build_info
                need_update_files.Add(new BundleInfo(XFABConst.project_build_info, 0, string.Empty));
            }
            result.need_update_bundles = need_update_files.ToArray();
        }


        /// <summary>
        /// 检测本地资源
        /// </summary>
        private IEnumerator CheckLocalRes(string projectName)
        {
            bool isUpdate = AssetBundleManager.GetProfile(projectName).updateModel == UpdateMode.UPDATE;

            // 如果是编辑器  并且 从 Assets 加载资源 
            // 这种情况下 是 不需要AssetBundle的 
#if UNITY_EDITOR
            if (AssetBundleManager.GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                result.updateType = UpdateType.DontNeedUpdate;
                yield break;
            }
#endif

            // 判断是否有内置资源
            IsHaveBuiltInResRequest isHaveBuiltIn = AssetBundleManager.IsHaveBuiltInRes(projectName);
            yield return isHaveBuiltIn;

            // 判断本地是否资源 
            if (AssetBundleManager.IsHaveResOnLocal(projectName))
            {
                // 本地有资源 
                //result.updateType = isUpdate ? UpdateType.Update : UpdateType.DontNeedUpdate;
                if (isUpdate)
                {
                    result.updateType = UpdateType.Update;
                }
                else
                {
                    if (isHaveBuiltIn.isHave/* && isHaveBuiltIn.buildInResType == BuildInResType.Files*/)
                    {
                        // 如果是非更新模式 且本地有资源 判断是否需要释放 判断版本号是否相同

#if UNITY_ANDROID && !UNITY_EDITOR
                        UnityWebRequest requestFile = UnityWebRequest.Get(XFABTools.BuildInDataPath(projectName, XFABConst.project_build_info));
                        yield return requestFile.SendWebRequest();
                        ProjectBuildInfo projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(requestFile.downloadHandler.text);
#else
                        ProjectBuildInfo projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(File.ReadAllText(XFABTools.BuildInDataPath(projectName, XFABConst.project_build_info)));
#endif

                        bool isNeedExtract = !projectBuildInfo.version.Equals(XFABTools.LocalResVersion(projectName));
                        //// 获取到内置文件列表 与本地进行比较 判断是否需要释放
                        //foreach (var bundleInfo in projectBuildInfo.bundleInfos)
                        //{
                        //    string buildInRes = XFABTools.BuildInDataPath(result.projectName, bundleInfo.bundleName);
                        //    string localRes = XFABTools.LocalResPath(result.projectName, bundleInfo.bundleName);

                        //    if (!File.Exists(localRes) )
                        //    {
                        //        isNeedExtract = true;
                        //        break;
                        //    }

                        //    // 判断md5的值是否相同
                        //    Task<string> local_md5 = XFABTools.md5fileasync(localRes);

                        //    while (!local_md5.IsCompleted)
                        //    {
                        //        yield return null;
                        //    }

                        //    if ( !bundleInfo.md5.Equals(local_md5.Result) ) {
                        //        isNeedExtract = true;
                        //        break;
                        //    }

                        //}
                        result.updateType = isNeedExtract ? UpdateType.ExtractLocal : UpdateType.DontNeedUpdate;

                    }
                    else
                    {
                        result.updateType = UpdateType.DontNeedUpdate;
                    }
                }
            }
            else
            {
                // 如果没有 判断有没有内置资源 如果有 就释放 如果没有 从服务器下载

                if (isHaveBuiltIn.isHave)
                {
                    // 判断是不是内置的压缩资源
                    result.updateType = /*(isHaveBuiltIn.buildInResType == BuildInResType.Zip ? UpdateType.ExtractLocalZip :*/ UpdateType.ExtractLocal; // 释放资源
                }
                else
                {
                    result.updateType = isUpdate ? UpdateType.Download : UpdateType.Error;
                    if (!isUpdate)
                    {
                        result.message = string.Format("缺少{0}资源！", projectName);
                    }
                }
            }
        }

    }


    /// <summary>
    /// 更新类型 
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// 更新资源
        /// </summary>
        Update,
        /// <summary>
        /// 下载资源
        /// </summary>
        Download,

        /// <summary>
        /// 下载压缩文件
        /// </summary>
        DownloadZip,

        /// <summary>
        /// 释放内置资源
        /// </summary>
        ExtractLocal,

        /// <summary>
        /// 释放内置压缩资源
        /// </summary>
        [Obsolete("删除释放压缩资源选项!", true)]
        ExtractLocalZip,

        /// <summary>
        /// 不需要更新
        /// </summary>
        DontNeedUpdate,
        /// <summary>
        /// 更新出错
        /// </summary>
        Error

    }





}

