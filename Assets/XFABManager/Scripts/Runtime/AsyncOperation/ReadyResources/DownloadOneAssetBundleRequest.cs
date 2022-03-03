using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace XFABManager
{
    /// <summary>
    /// 下载某个项目中的某一个AssetBundle
    /// </summary>
    /// 
    public class DownloadOneAssetBundleRequest : CustomAsyncOperation<DownloadOneAssetBundleRequest>
    {

        /// <summary>
        /// 当前的下载速度 单位 字节
        /// </summary>
        public long CurrentSpeed
        {
            get; protected set;
        }

        public UpdateType updateType { get; private set; }

        private static Dictionary<string, ProjectBuildInfo> build_infos = new Dictionary<string, ProjectBuildInfo>();

        internal IEnumerator DownloadOneAssetBundle(string projectName, string bundleName)
        {
            if (!AssetBundleManager.isInited)
            {
                // 初始化
                yield return AssetBundleManager.InitializeAsync();
            }

#if UNITY_EDITOR
            if (AssetBundleManager.GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                // 如果是编辑器模式 并且从 Assets中加载资源 没有必要 下载 AssetBundle ,直接完成即可!
                Completed();
                yield break;
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            // 判断更新模式 
            if (AssetBundleManager.GetProfile(projectName).updateModel == UpdateMode.LOCAL) {
                yield return CoroutineStarter.Start(this.ReadyAssetBundleFromLocal(projectName, bundle_name));
            } else{
                yield return CoroutineStarter.Start(this.ReadyAssetBundleFromNet(projectName, bundle_name));
            }
            
        }

        private bool IsNeedUpdate(string localfile, string server_md5)
        {

            bool isUpdate = false;
            if (File.Exists(localfile))
            {
                if (!server_md5.Equals(XFABTools.md5file(localfile)))
                {
                    // 进行更新
                    isUpdate = true;
                    File.Delete(localfile);
                }
            }
            else
            {
                isUpdate = true;
            }
            return isUpdate;
        }

        private IEnumerator ReadyAssetBundleFromNet(string projectName, string bundleName) {

            // 获取版本
            GetProjectVersionRequest requestVer = AssetBundleManager.GetProjectVersion(projectName);
            yield return requestVer;

            if (!string.IsNullOrEmpty(requestVer.error))
            {
                Completed(string.Format("获取{0}版本出错:{1}", projectName, requestVer.error));
                yield break;
            }

            // 下载 project_build_info 文件
            string project_build_server = XFABTools.ServerPath(AssetBundleManager.GetProfile(projectName).url, projectName, requestVer.version, XFABConst.project_build_info);
            string project_build_local = XFABTools.LocalResPath(projectName, XFABConst.project_build_info);
            ProjectBuildInfo projectBuildInfo = null;

            if (!build_infos.ContainsKey(project_build_server))
            {
                DownloadFileRequest requestBuild = DownloadFileRequest.Download(project_build_server, project_build_local);
                yield return requestBuild;
                if (!string.IsNullOrEmpty(requestBuild.error))
                {
                    Completed(string.Format("获取{0}: project_build_info.json 出错:{1}", projectName, requestBuild.error));
                    yield break;
                }
                projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(File.ReadAllText(project_build_local));
                if (!build_infos.ContainsKey(project_build_server))
                    build_infos.Add(project_build_server, projectBuildInfo);
            }
            else {
                projectBuildInfo = build_infos[project_build_server];
            }

            // 下载 bundle 依赖信息 bundle
            string bundle_dependon_file_server = XFABTools.ServerPath(AssetBundleManager.GetProfile(projectName).url, projectName, requestVer.version, XFABTools.GetCurrentPlatformName());
            string bundle_dependon_file_local = XFABTools.LocalResPath(projectName, XFABTools.GetCurrentPlatformName());

            string bundle_dependon_file_md5 = null;

            for (int i = 0; i < projectBuildInfo.bundleInfos.Length; i++)
            {
                if (projectBuildInfo.bundleInfos[i].bundleName.Equals(XFABTools.GetCurrentPlatformName()))
                {
                    bundle_dependon_file_md5 = projectBuildInfo.bundleInfos[i].md5;
                    break;
                }
            }

            bool isNeedDownload = IsNeedUpdate(bundle_dependon_file_local, bundle_dependon_file_md5);

            if (isNeedDownload)
            {
                DownloadFileRequest requestDependonBundle = DownloadFileRequest.Download(bundle_dependon_file_server, bundle_dependon_file_local);
                yield return requestDependonBundle;
                if (!string.IsNullOrEmpty(requestDependonBundle.error))
                {
                    Completed(string.Format("下载{0}:出错:", bundle_dependon_file_server));
                    yield break;
                }
            }

            //DownloadFilesRequest downloadFiles = new DownloadFilesRequest();
            List<DownloadObjectInfo> need_down_files = CheckNeedDownloadRes(projectName, bundleName, projectBuildInfo, requestVer.version);
            // 获取到Bundle的依赖
            //downloadFiles.AddRange( CheckNeedDownloadRes(projectName, bundleName, projectBuildInfo, requestVer.version));

            if (need_down_files.Count != 0)
            {

                updateType = UpdateType.Update;

                DownloadFilesRequest downloadFiles = DownloadFilesRequest.DownloadFiles(need_down_files);
                while (!downloadFiles.isDone)
                {
                    yield return null;
                    progress = downloadFiles.progress;
                    CurrentSpeed = downloadFiles.Speed;
                }

                if (!string.IsNullOrEmpty(downloadFiles.error))
                {
                    Completed(downloadFiles.error);
                    yield break;
                }

            }
            else {
                updateType = UpdateType.DontNeedUpdate;
            } 

            // 验证文件是否正确 
            if (CheckNeedDownloadRes(projectName, bundleName, projectBuildInfo, requestVer.version).Count != 0) {
                // 说明文件不对 需要重新下载
                yield return CoroutineStarter.Start(this.ReadyAssetBundleFromNet(projectName, bundleName));
            }


            Completed();
        }

        private IEnumerator ReadyAssetBundleFromLocal(string projectName, string bundleName) {

            updateType = UpdateType.ExtractLocal;

            // 判断有没有内置资源
            IsHaveBuiltInResRequest buildIn = AssetBundleManager.IsHaveBuiltInRes(projectName);
            yield return buildIn;
            // 有内置
            if (buildIn.isHave /*&& buildIn.buildInResType == BuildInResType.Files*/)
            {
                // 获取 .json 文件
                string project_build_in = XFABTools.BuildInDataPath(projectName, XFABConst.project_build_info);
                string project_build_local = XFABTools.LocalResPath(projectName, XFABConst.project_build_info);
                // 复制到数据目录

                CopyFileRequest copy_build_request = FileTools.Copy(project_build_in, project_build_local);
                yield return copy_build_request;
                if ( !string.IsNullOrEmpty(copy_build_request.error) ) {
                    Completed(copy_build_request.error);
                    yield break;
                }

                ProjectBuildInfo projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(File.ReadAllText(project_build_local));
                // 获取到后缀
                string fileName = string.Format("{0}{1}", bundleName, projectBuildInfo.suffix);
                // 获取依赖Bundle

                string dependeceBundlePath = XFABTools.BuildInDataPath(projectName, XFABTools.GetCurrentPlatformName());
                string dependeceBundlePathLocal = XFABTools.LocalResPath(projectName, XFABTools.GetCurrentPlatformName());

                // 复制到数据目录
                CopyFileRequest copy_dependence_request = FileTools.Copy(dependeceBundlePath, dependeceBundlePathLocal);
                yield return copy_dependence_request;
                if (!string.IsNullOrEmpty(copy_dependence_request.error))
                {
                    Completed(copy_dependence_request.error);
                    yield break;
                }

                UnloadDependenceBundle();

                AssetBundleManager.DependenceBundle = AssetBundle.LoadFromFile(dependeceBundlePathLocal);
                AssetBundleManifest manifest = AssetBundleManager.DependenceBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                
                
                string[] dependences = manifest.GetAllDependencies(fileName);

                // 复制依赖的AssetBundle 
                for (int i = 0; i < dependences.Length; i++)
                {
                    yield return CoroutineStarter.Start( CopyAssetBundle(projectName, dependences[i].ToLower()));
                }

                // 复制AssetBundle
                yield return CoroutineStarter.Start( CopyAssetBundle(projectName, fileName.ToLower()));

                UnloadDependenceBundle();

                Completed();
            }
            else
            {
                Completed("缺少资源:" + projectName);
            }
 
        }

        private void UnloadDependenceBundle() {
            if (AssetBundleManager.DependenceBundle != null)
            {
                AssetBundleManager.DependenceBundle.Unload(true);
                AssetBundleManager.DependenceBundle = null;
            }
        }

        /// <summary>
        /// 含后缀
        /// </summary>
        /// <param name="fileName"></param>
        private IEnumerator CopyAssetBundle(string projectName,string fileName) {

            string from = XFABTools.BuildInDataPath(projectName, fileName);
            string to = XFABTools.LocalResPath(projectName, fileName);

            //Debug.Log(fileName);

            // 判断本地有没有文件 
            if (!File.Exists(to))
            {
                // 没有文件 直接复制
                yield return FileTools.Copy(from, to);
            }
            else
            {

#if UNITY_ANDROID && !UNITY_EDITOR
                UnityWebRequest requestFile = UnityWebRequest.Get(XFABTools.BuildInDataPath(projectName, XFABConst.project_build_info));
                yield return requestFile.SendWebRequest();
                ProjectBuildInfo projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(requestFile.downloadHandler.text);
#else
                ProjectBuildInfo projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(File.ReadAllText(XFABTools.BuildInDataPath(projectName, XFABConst.project_build_info)));
#endif

                BundleInfo bundleInfo = projectBuildInfo.bundleInfos.Where(x => x.bundleName.Equals(fileName)).FirstOrDefault();
                if ( bundleInfo != null )
                {

                    //Task<string> toMD5 = XFABTools.md5fileasync(to);

                    //while (!toMD5.IsCompleted)
                    //{
                    //    yield return null;
                    //}


                    FileMD5Request fileMD5 = XFABTools.CaculateFileMD5(to);
                    yield return fileMD5;

                    if (!string.IsNullOrEmpty(fileMD5.error))
                    {
                        Completed(string.Format("计算md5失败:{0} error:{1}", to, fileMD5.error));
                        yield break;
                    }

                    // 有文件 判断md5值 需不需要更新
                    if (fileMD5.md5 != bundleInfo.md5)
                    {
                        File.Delete(to);
                        yield return FileTools.Copy(from, to);
                    }
                }

            }



        }

        /// <summary>
        /// 检测需要下载的资源
        /// </summary>
        private List<DownloadObjectInfo> CheckNeedDownloadRes(string projectName,string bundleName, ProjectBuildInfo projectBuildInfo,string version)
        {

            //Dictionary<string, string> need_download_bundles = new Dictionary<string, string>();
            List<DownloadObjectInfo> need_download_bundles = new List<DownloadObjectInfo>();
            string[] dependeces = AssetBundleManager.GetAssetBundleDependences(projectName, bundleName);
            List<string> need_check_bundles = new List<string>(dependeces.Length + 1);


            need_check_bundles.Add(bundleName);
            need_check_bundles.AddRange(dependeces);

            for (int i = 0; i < need_check_bundles.Count; i++)
            {

                string fileName = string.Format("{0}{1}", need_check_bundles[i].ToLower(), projectBuildInfo.suffix);

                string localfile = XFABTools.LocalResPath(projectName, fileName);
                string server_url = XFABTools.ServerPath(AssetBundleManager.GetProfile(projectName).url, projectName, version, fileName);

                BundleInfo bundleInfo = projectBuildInfo.bundleInfos.Where(x => x.bundleName.Equals(fileName)).FirstOrDefault();
                if (bundleInfo != null)
                {
                    if (IsNeedUpdate(localfile, bundleInfo.md5))
                    {
                        need_download_bundles.Add(new DownloadObjectInfo(server_url, localfile, bundleInfo.bundleSize));
                    }
                }
                else {
                    Completed(string.Format("未在服务端配置中发现文件:{0}",fileName));
                    need_download_bundles.Clear();
                    return need_download_bundles;
                }


                //for (int j = 0; j < projectBuildInfo.bundleInfos.Length; j++)
                //{
                //    if (fileName.Equals(projectBuildInfo.bundleInfos[j].bundleName))
                //    {
                //        if (IsNeedUpdate(localfile, projectBuildInfo.bundleInfos[j].md5))
                //        {
                //            need_download_bundles.Add( new DownloadObjectInfo( server_url, localfile, projectBuildInfo.bundleInfos[j].bundleSize));
                //        }
                //        break;
                //    }
                //}
            }

            return need_download_bundles;
        }

    }

}

