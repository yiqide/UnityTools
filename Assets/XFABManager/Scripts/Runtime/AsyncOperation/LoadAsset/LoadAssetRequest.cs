using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFABManager
{

    public class LoadAssetRequest : CustomAsyncOperation<LoadAssetRequest>
    {

        //private AssetBundleManager bundleManager;

        //private UnityEngine.Object _asset;

        public UnityEngine.Object asset
        {
            get; protected set;
        }

        //public LoadAssetRequest(AssetBundleManager bundleManager) {
        //    this.bundleManager = bundleManager;
        //}

        // 异步加载资源 
        internal IEnumerator LoadAssetAsync<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object
        {

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName))
            {
                yield return new WaitForEndOfFrame();
                Completed(string.Format("项目名 bundle名 或 资源名为空! projectName:{0} bundleName:{1} assetName:{2} ",projectName,bundleName,assetName));
                yield break;
            }

#if UNITY_EDITOR
            if (AssetBundleManager.GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                yield return null;
                asset = AssetBundleManager.LoadAsset<T>(projectName, bundleName, assetName);
                Completed();
                yield break;
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            LoadAssetBundleRequest requestBundle = AssetBundleManager.LoadAssetBundleAsync(projectName, bundle_name);
            yield return requestBundle;

            if (!string.IsNullOrEmpty(requestBundle.error))
            {
                Completed(string.Format("加载AssetBundle:{0}/{1}出错:{2}", projectName, bundle_name, requestBundle.error));
                yield break;
            }
            AssetBundleRequest requestAsset = requestBundle.assetBundle.LoadAssetAsync<T>(assetName);
            yield return requestAsset;
            if (requestAsset != null && requestAsset.asset != null)
            {
                asset = requestAsset.asset;
            }
            else {
                error = string.Format("资源{0}/{1}/{2}加载失败!",projectName, bundle_name, assetName);
            }
            Completed();
        }

        internal IEnumerator LoadAssetAsync(string projectName, string bundleName, string assetName, Type type )
        {
            // 防空判断
            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName))
            {
                yield return new WaitForEndOfFrame();
                Completed(string.Format("项目名 bundle名 或 资源名为空! projectName:{0} bundleName:{1} assetName:{2} ", projectName, bundleName, assetName));
                yield break;
            }

#if UNITY_EDITOR
            if (AssetBundleManager.GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                yield return null;
                asset = AssetBundleManager.LoadAsset(projectName, bundleName, assetName,type);
                Completed();
                yield break;
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            LoadAssetBundleRequest requestBundle = AssetBundleManager.LoadAssetBundleAsync(projectName, bundle_name);
            yield return requestBundle;
            if (!string.IsNullOrEmpty(requestBundle.error))
            {
                Completed(string.Format("加载AssetBundle:{0}/{1}出错:{2}", projectName, bundle_name, requestBundle.error));
                yield break;
            }
            AssetBundleRequest requestAsset = requestBundle.assetBundle.LoadAssetAsync(assetName, type);
            yield return requestAsset;
            if (requestAsset != null && requestAsset.asset != null)
            {
                asset = requestAsset.asset;
            }
            else
            {
                error = string.Format("资源{0}/{1}/{2}加载失败!", projectName, bundle_name, assetName);
            }
            Completed();

        }
    }
}

