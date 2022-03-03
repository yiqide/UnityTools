using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFABManager
{
    public class LoadAssetBundleRequest : CustomAsyncOperation<LoadAssetBundleRequest>
    {
        //private AssetBundleManager bundleManager;
        //private AssetBundle _assetBundle;
        public AssetBundle assetBundle
        {
            get; protected set;
        }

        //public LoadAssetBundleRequest(AssetBundleManager bundleManager) {
        //    this.bundleManager = bundleManager;
        //}

        internal IEnumerator LoadAssetBundle(string projectName, string bundleName)
        {
            bundleName = bundleName.ToLower();
            // 判断是否 已经有这个模块的资源
            if (!AssetBundleManager.AssetBundles.ContainsKey(projectName))
            {
                AssetBundleManager.AssetBundles.Add(projectName, new Dictionary<string, AssetBundle>());
            }
            // 判断是否已经加载了这个AssetBundle
            if (AssetBundleManager.IsLoadedAssetBundle(projectName, bundleName))
            {
                assetBundle = AssetBundleManager.AssetBundles[projectName][bundleName];
                Completed();
                yield break;
            }
            string suffix = AssetBundleManager.GetAssetBundleSuffix(projectName);
            string[] dependences = AssetBundleManager.GetAssetBundleDependences(projectName, bundleName);

            List<string> need_load_bundle = new List<string>(dependences.Length +1);
            need_load_bundle.Add(bundleName);        // 加载自己
            need_load_bundle.AddRange(dependences);  // 加载依赖项目

            for (int i = 0; i < need_load_bundle.Count; i++)
            {

                if (AssetBundleManager.IsLoadedAssetBundle(projectName, need_load_bundle[i])) {
                    continue;
                }
                string bundlePath = AssetBundleManager.GetAssetBundlePath(projectName, need_load_bundle[i], suffix);
                AssetBundleCreateRequest request = AssetBundleManager.LoadAssetBundleAsync(bundlePath);
                yield return request;

                if (request != null && request.assetBundle != null)
                {
                    // 加载成功
                    AssetBundleManager.AssetBundles[projectName].Add(need_load_bundle[i], request.assetBundle);
                }
                else {
                    Completed(string.Format("AssetBundle:{0}加载失败!", bundlePath));
                    yield break;
                }
                progress = (float)(i + 1) / need_load_bundle.Count;
            }
            assetBundle = AssetBundleManager.AssetBundles[projectName][bundleName];
            Completed();
        }
    }

}

