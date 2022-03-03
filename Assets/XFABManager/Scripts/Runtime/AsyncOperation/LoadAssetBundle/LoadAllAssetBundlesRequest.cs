using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace XFABManager
{
    public class LoadAllAssetBundlesRequest : CustomAsyncOperation<LoadAllAssetBundlesRequest>
    {
        //private AssetBundleManager bundleManager;

        //public LoadAllAssetBundlesRequest(AssetBundleManager bundleManager) {
        //    this.bundleManager = bundleManager;
        //}

        internal IEnumerator LoadAllAssetBundles(string projectName )
        {
#if UNITY_EDITOR
            if (AssetBundleManager.GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                yield return null;
                // 如果是 编辑器模式 并且从 Assets 加载资源 可以直接完成
                Completed();
                yield break;
            }
#endif

            // 读取这个模块所有的文件
            string project_build_info = XFABTools.LocalResPath(projectName, XFABConst.project_build_info);
            if (!File.Exists(project_build_info))
            {
                Completed(string.Format("LoadAllAssetBundles 失败!{0}文件不存在", project_build_info));
                yield break;
            }

            ProjectBuildInfo buildInfo = JsonUtility.FromJson<ProjectBuildInfo>( File.ReadAllText(project_build_info));
            //string suffix = buildInfo.suffix;
            for (int i = 0; i < buildInfo.bundleInfos.Length; i++)
            {
                string bundleName = Path.GetFileNameWithoutExtension(buildInfo.bundleInfos[i].bundleName);

                if (bundleName.Equals( XFABTools.GetCurrentPlatformName() )) {
                    continue;
                }

                yield return AssetBundleManager.LoadAssetBundleAsync(projectName, bundleName);
                progress = (float)i / buildInfo.bundleInfos.Length;
            }

            Completed();
        }
    }
}


