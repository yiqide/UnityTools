using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace XFABManager
{
    public class InitializeRequest : CustomAsyncOperation<InitializeRequest>
    {

        internal IEnumerator Initialize()
        {
            yield return null;
#if UNITY_EDITOR
            AssetBundleManager.profiles = XFABManagerSettings.Settings.CurrentProfiles;
            AssetBundleManager.isInited = true;

#else
            string profilePath = string.Format("{0}/{1}", Application.streamingAssetsPath, XFABConst.profile_setting);
#if UNITY_ANDROID
            UnityWebRequest request = UnityWebRequest.Get(profilePath);
            yield return request.SendWebRequest();
            AssetBundleManager.profiles = JsonUtility.FromJson<ProfileConfig>(request.downloadHandler.text).profiles;
#else
                if (File.Exists(profilePath))
                {
                    AssetBundleManager.profiles = JsonUtility.FromJson<ProfileConfig>(File.ReadAllText(profilePath)).profiles;
                }
                else
                {
                    throw new Exception(string.Format("配置文件:{0}不存在!", profilePath));
                }
#endif    

#endif
            // 初始化 获取项目版本接口
            if (AssetBundleManager.GetProfile().useDefaultGetProjectVersion)
            {
                //GameObject gameObject = new GameObject("GetProjectVersionDefault");
                //GameObject.DontDestroyOnLoad(gameObject);
                //GetProjectVersionDefault defaultGetVersion = gameObject.AddComponent<GetProjectVersionDefault>();
                //defaultGetVersion.url = AssetBundleManager.GetProfile().url;
                AssetBundleManager.SetGetProjectVersion<GetProjectVersionDefault>();
            }

            // 初始化服务器文件路径接口
            AssetBundleManager.SetServerFilePath(new DefaultServerFilePath());

            AssetBundleManager.isInited = true;
            Completed();
        }


    }
}


