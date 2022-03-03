using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace XFABManager
{
    public class ExtractResRequest : CustomAsyncOperation<ExtractResRequest>
    {
         
        public string CurrentExtractFile { get; private set; }

        internal IEnumerator ExtractRes(string projectName) {

            CheckResUpdateRequest checkUpdate = AssetBundleManager.CheckResUpdate(projectName);
            yield return checkUpdate;

            if ( !string.IsNullOrEmpty(checkUpdate.error) ) {
                Completed(checkUpdate.error);
                yield break;
            }

            yield return CoroutineStarter.Start(ExtractRes(checkUpdate.result));

        }


        internal IEnumerator ExtractRes( CheckUpdateResult result)
        {

            // 判断本地是否有这个模块资源 如果有了就不用释放了
            if (/*AssetBundleManager.IsHaveResOnLocal(result.projectName) ||*/ result.updateType == UpdateType.DontNeedUpdate )
            {
                //Debug.LogWarningFormat("模块{0}资源已经释放了!",result.projectName);
                Completed();
                yield break;
            }

            // 判断是否有内置资源
            IsHaveBuiltInResRequest isHaveBuiltInRes = AssetBundleManager.IsHaveBuiltInRes(result.projectName);
            yield return isHaveBuiltInRes;
            if (!isHaveBuiltInRes.isHave)
            {
                Completed(string.Format("释放资源失败!未查询到内置资源{0}!", result.projectName));
                yield break;
            }

            // 释放压缩资源
            string buildInRes = null;
            string localRes = null;


            switch (result.updateType)
            {
                case UpdateType.ExtractLocal:
                    string project_build_info = XFABTools.BuildInDataPath(result.projectName, XFABConst.project_build_info);

#if UNITY_ANDROID && !UNITY_EDITOR
                    UnityWebRequest requestBuildInfo = UnityWebRequest.Get(project_build_info);
                    yield return requestBuildInfo.SendWebRequest();
                    ProjectBuildInfo buildInfo = JsonUtility.FromJson<ProjectBuildInfo>(requestBuildInfo.downloadHandler.text);
#else
                    ProjectBuildInfo buildInfo = JsonUtility.FromJson<ProjectBuildInfo>(File.ReadAllText(project_build_info));
#endif

                    // 清空本地资源 
                    AssetBundleManager.ClearProjectCache(result.projectName);

                    for (int i = 0; i < buildInfo.bundleInfos.Length; i++)
                    {
                        BundleInfo bundleInfo = buildInfo.bundleInfos[i];

                        buildInRes = XFABTools.BuildInDataPath(result.projectName, bundleInfo.bundleName);
                        localRes = XFABTools.LocalResPath(result.projectName, bundleInfo.bundleName);

                        //if (File.Exists(localRes) ) {
                        //    Task<string> local_md5 = XFABTools.md5fileasync(localRes);

                        //    while (!local_md5.IsCompleted)
                        //    {
                        //        yield return null;
                        //    }

                        //    if (bundleInfo.md5.Equals(local_md5.Result)) { 
                        //        continue;
                        //    }
                        //}

                        CopyFileRequest copy_buildin_request = FileTools.Copy(buildInRes, localRes);
                        yield return copy_buildin_request; 

                        if (!string.IsNullOrEmpty(copy_buildin_request.error)) {
                            Completed(copy_buildin_request.error);
                            yield break;
                        }

                        CurrentExtractFile = bundleInfo.bundleName;
                        progress = (float)(i+1) / buildInfo.bundleInfos.Length;

                        //yield return null;
                    }


                    // 释放 files 文件列表文件 
                    CopyFileRequest copy_build_info_request = FileTools.Copy(project_build_info, XFABTools.LocalResPath(result.projectName, XFABConst.project_build_info));
                    yield return copy_build_info_request;
                    if (!string.IsNullOrEmpty(copy_build_info_request.error))
                    {
                        Completed(copy_build_info_request.error);
                        yield break;
                    }
                    
                    // 更新版本号
                    XFABTools.SaveVersion(result.projectName, buildInfo.version);

                    break;
                //case UpdateType.ExtractLocalZip:
                //    string fileName = string.Format("{0}{1}", result.projectName, ".zip");
                //    // 释放压缩资源
                //    buildInRes = XFABTools.BuildInDataPath(result.projectName, fileName);
                //    localRes = XFABTools.LocalResPath(result.projectName, fileName);
                //    CopyFileRequest copy_zip_request = FileTools.Copy(buildInRes, localRes);
                //    yield return copy_zip_request;
                //    if (!string.IsNullOrEmpty(copy_zip_request.error))
                //    {
                //        Completed(copy_zip_request.error);
                //        yield break;
                //    }

                //    CurrentExtractFile = "正在解压资源...";
                //    progress = 1;
                //    // 解压 
                //    ZipTools.UnZipFile(localRes, XFABTools.DataPath(result.projectName));
                //    yield return null;
                //    // 解压完成 删除压缩包
                //    File.Delete(localRes);
                    //buildInfo = JsonUtility.FromJson<ProjectBuildInfo>(File.ReadAllText(project_build_info));

                    //break;
                default:
                    Debug.LogErrorFormat("释放资源出错,未知类型{0}", result.updateType);
                    break;
            }

            Completed();
        }
    }

}

