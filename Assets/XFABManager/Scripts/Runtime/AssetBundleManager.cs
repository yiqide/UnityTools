using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
//using UnityEngine.SceneManagement;

namespace XFABManager
{

    public class AssetBundleManager 
    {


        #region 字段 

        //private static Profile _profile;

        /// <summary>
        /// 存放所有的 AssetBundle
        /// </summary>
        private static Dictionary<string, Dictionary<string, AssetBundle>> assetBundles = new Dictionary<string, Dictionary<string, AssetBundle>>();

        /// <summary>
        /// 保存各个模块的后缀名
        /// </summary>
        private static Dictionary<string, string> suffixs = new Dictionary<string, string>();

        private static Type getProjectVersion;

        //private static Dictionary<string, IGetProjectVersion> get_project_version_caches = new Dictionary<string, IGetProjectVersion>();

        public static IServerFilePath ServerFilePath { get;private set; }

        private static AssetBundle dependenceBundle = null;
        private static string dependenceProjectName = string.Empty;

        internal static bool isInited = false;

        /// <summary>
        /// 不能重复的异步操作的缓存 
        /// </summary>
        private static Dictionary<string, CustomYieldInstruction> async_cache = new Dictionary<string, CustomYieldInstruction>();

        public delegate CustomYieldInstruction AsyncOperationDelegate();

        //public event AsyncOperationDelegate ExecuteAsync;
        private static Profile[] _profiles;
        internal static Profile[] profiles 
        {
            get {
#if UNITY_EDITOR
                if (_profiles == null) {
                    _profiles = XFABManagerSettings.Settings.CurrentProfiles;
                }
#endif
                return _profiles;
            }
            set {
                _profiles = value;
            }
        }
        #endregion

        #region 属性

        /// <summary>
        /// 保存加载的所有AssetBundle
        /// </summary>
        public static Dictionary<string, Dictionary<string, AssetBundle>> AssetBundles
        {
            get { return assetBundles; }
        }

        // 获取依赖的AssetBundle
        internal static AssetBundle DependenceBundle
        {
            get
            {
                return dependenceBundle;
            }
            set
            {
                dependenceBundle = value;
            }
        }

        #endregion


        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static InitializeRequest InitializeAsync()
        {
            return ExecuteOnlyOnceAtATime<InitializeRequest>("Initialize", () =>
            {
                if (isInited) { return null; }
                // 开始初始化
                InitializeRequest initializeRequest = new InitializeRequest();
                CoroutineStarter.Start(initializeRequest.Initialize());
                return initializeRequest;
            });
        }

        #region 更新和下载资源

        /// <summary>
        /// 准备某个模块的资源 
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static ReadyResRequest ReadyRes(string projectName)
        {
            //string key = string.Format("ReadyResRequest:{0}", projectName);
            //return ExecuteOnlyOnceAtATime<ReadyResRequest>(key, () =>
            //{
                ReadyResRequest request = new ReadyResRequest();
                CoroutineStarter.Start(request.ReadyRes(projectName));
                return request;
            //});

        }


        /// <summary>
        /// 准备 某一组检测结果的资源
        /// </summary>
        /// <param name="results">检测结果</param>
        /// <returns></returns>
        public static ReadyResRequest ReadyRes(CheckUpdateResult[] results) {
            ReadyResRequest request = new ReadyResRequest();
            CoroutineStarter.Start(request.ReadyRes(results));
            return request;
        }

        /// <summary>
        /// 检测某个项目 及其依赖项目 是否需要更新 下载 或者 释放 等等
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static CheckResUpdatesRequest CheckResUpdates(string projectName)
        {
            CheckResUpdatesRequest request = new CheckResUpdatesRequest();
            CoroutineStarter.Start(request.CheckResUpdates(projectName));
            return request;
        }

        /// <summary>
        /// 检测某个项目 是否需要更新 下载 或者 释放 等等
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static CheckResUpdateRequest CheckResUpdate(string projectName)
        {
            CheckResUpdateRequest request = new CheckResUpdateRequest();
            CoroutineStarter.Start(request.CheckResUpdate(projectName));
            return request;
        }


        /// <summary>
        /// 从服务端获取文件 
        /// </summary>
        /// <param name="projectName">项目名</param>
        /// <param name="version">版本</param>
        /// <param name="fileName">文件名 含后缀</param>
        internal static GetFileFromServerRequest GetFileFromServer(string projectName, string version, string fileName)
        {
            string key = string.Format("GetFileFromServer:{0}{1}{2}", projectName, version, fileName);
            return ExecuteOnlyOnceAtATime<GetFileFromServerRequest>(key, () =>
            {
                GetFileFromServerRequest request = new GetFileFromServerRequest(GetProfile(projectName).url);
                CoroutineStarter.Start(request.GetFileFromServer(projectName, version, fileName));
                return request;
            });
        }

        /// <summary>
        /// 更新或下载资源
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static UpdateOrDownloadResRequest UpdateOrDownloadRes(string projectName)
        {
            string key = string.Format("UpdateOrDownloadResRequest:{0}", projectName);
            return ExecuteOnlyOnceAtATime<UpdateOrDownloadResRequest>(key, () =>
            {
                UpdateOrDownloadResRequest request = new UpdateOrDownloadResRequest();
                CoroutineStarter.Start(request.UpdateOrDownloadRes(projectName));
                return request;
            });
        }


        /// <summary>
        /// 更新 或 下载资源  
        /// </summary>
        public static UpdateOrDownloadResRequest UpdateOrDownloadRes(CheckUpdateResult result)
        {
            string key = string.Format("UpdateOrDownloadResRequest:{0}", result.projectName);
            return ExecuteOnlyOnceAtATime<UpdateOrDownloadResRequest>(key, () =>
            {
                UpdateOrDownloadResRequest request = new UpdateOrDownloadResRequest();
                CoroutineStarter.Start(request.UpdateOrDownloadRes(result));
                return request;
            });
        }



        public static DownloadOneAssetBundleRequest DownloadOneAssetBundle(string projectName, string bundleName)
        {
            string key = string.Format("DownloadOneAssetBundle:{0}{1}", projectName,bundleName);
            return ExecuteOnlyOnceAtATime<DownloadOneAssetBundleRequest>(key, () =>
            {
                DownloadOneAssetBundleRequest request = new DownloadOneAssetBundleRequest();
                CoroutineStarter.Start(request.DownloadOneAssetBundle(projectName, bundleName));
                return request;
            });
        }

        internal static GetProjectVersionRequest GetProjectVersion(string projectName)
        {
            string key = string.Format("GetProjectVersion:{0}", projectName);
            return ExecuteOnlyOnceAtATime<GetProjectVersionRequest>(key, () =>
            {
                GetProjectVersionRequest request = new GetProjectVersionRequest(CreateProjectVersionInstance());
                CoroutineStarter.Start(request.GetProjectVersion(projectName, GetProfile(projectName).updateModel));
                return request;
            });
        }

        /// <summary>
        /// 释放AssetBunle ( 从 StreamingAssets 到 persistentDataPath ) 
        /// 仅释放当前项目资源 不包含其依赖项目
        /// </summary>
        /// <param name="projectName"></param>
        public static ExtractResRequest ExtractRes(string projectName)
        {
            string key = string.Format("ExtractRes:{0}", projectName);
            return ExecuteOnlyOnceAtATime<ExtractResRequest>(key, () =>
            {
                ExtractResRequest request = new ExtractResRequest();
                CoroutineStarter.Start(request.ExtractRes(projectName));
                return request;
            });
        }

        /// <summary>
        /// 释放AssetBunle ( 从 StreamingAssets 到 persistentDataPath ) 
        /// 仅释放当前项目资源 不包含其依赖项目
        /// </summary>
        /// <param name="projectName"></param>
        public static ExtractResRequest ExtractRes(CheckUpdateResult result)
        {

            string key = string.Format("ExtractRes:{0}", result.projectName);
            return ExecuteOnlyOnceAtATime<ExtractResRequest>(key, () =>
            {
                ExtractResRequest request = new ExtractResRequest();
                CoroutineStarter.Start(request.ExtractRes(result));
                return request;
            });


        }

#endregion

        #region 加载AssetBundle

        /// <summary>
        /// 获取 AssetBundle 的后缀名
        /// </summary>
        /// <param name="projectName"></param>
        public static string GetAssetBundleSuffix(string projectName)
        {
            if (suffixs.ContainsKey(projectName))
            {
                return suffixs[projectName];
            }
            else
            {
                string filePath = XFABTools.LocalResPath(projectName, XFABConst.project_build_info);
                if (File.Exists(filePath))
                {
                    ProjectBuildInfo projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(File.ReadAllText(filePath));
                    //if (suffix.Contains("\n")) { Debug.Log("读取后缀出错!"); }
                    suffixs.Add(projectName, projectBuildInfo.suffix);
                    return projectBuildInfo.suffix;
                }
                //else
                //{
                //    Debug.LogErrorFormat("获取后缀失败!{0}不存在!", filePath);
                //}
            }
            return string.Empty;
        }

        internal static AssetBundle LoadAssetBundle(string bundle_path)
        {
            if (!File.Exists(bundle_path)) return null;
            AssetBundle bundle = AssetBundle.LoadFromFile(bundle_path);
            if (bundle == null)
            {
                Debug.LogErrorFormat("AssetBundle {0} 加载失败！", bundle_path);
            }
            return bundle;
        }

        /// <summary>
        /// 加载AssetBundle
        /// </summary>
        /// <param name="projectName">项目名</param>
        /// <param name="bundleName">bundle名 需要加后缀</param>
        internal static AssetBundle LoadAssetBundle(string projectName, string bundleName)
        {

            bundleName = bundleName.ToLower();
            //string bundle_name = string.Format("{0}_{1}", projectName.ToLower(), bundleName.ToLower());

            // 判断是否 已经有这个模块的资源
            if (!assetBundles.ContainsKey(projectName))
            {
                assetBundles.Add(projectName, new Dictionary<string, AssetBundle>());
            }

            // 判断是否已经加载了这个AssetBundle
            if (IsLoadedAssetBundle(projectName, bundleName))
            {
                return assetBundles[projectName][bundleName];
            }

            string[] dependences = GetAssetBundleDependences(projectName, bundleName);
            List<string> need_load_bundle = new List<string>(dependences.Length + 1);
            need_load_bundle.Add(bundleName);
            need_load_bundle.AddRange(dependences);

            for (int i = 0; i < need_load_bundle.Count; i++)
            {

                if (IsLoadedAssetBundle(projectName, need_load_bundle[i]))
                {
                    continue;
                }

                string bundlePath = GetAssetBundlePath(projectName, need_load_bundle[i], GetAssetBundleSuffix(projectName));
                AssetBundle bundle = LoadAssetBundle(bundlePath);

                if (bundle != null)
                {
                    // 加载成功
                    assetBundles[projectName].Add(need_load_bundle[i], bundle);
                }
            }

            AssetBundle ab;
            if ( assetBundles[projectName].TryGetValue(bundleName,out ab) ) {
                return ab;
            }

            return null;
        }

        internal static AssetBundleCreateRequest LoadAssetBundleAsync(string bundle_path)
        {
            if (!File.Exists(bundle_path)) return null;
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundle_path);
            if (request == null)
            {
                Debug.LogErrorFormat("AssetBundle {0} 加载失败！", bundle_path);
            }
            return request;
        }

        internal static LoadAssetBundleRequest LoadAssetBundleAsync(string projectName, string bundleName)
        {
            //Debug.LogFormat("异步加载AssetBundle:{0}/{1}", projectName, bundleName);
            string key = string.Format("LoadAssetBundleAsync:{0}{1}", projectName, bundleName);
            //string bundle_name = string.Format("{0}_{1}",projectName.ToLower(),bundleName.ToLower());
            return ExecuteOnlyOnceAtATime<LoadAssetBundleRequest>(key, () =>
            {
                LoadAssetBundleRequest request = new LoadAssetBundleRequest();
                CoroutineStarter.Start(request.LoadAssetBundle(projectName, bundleName));
                return request;
            });
        }

        /// <summary>
        /// 判断是否已经加载 某个AssetBundle
        /// 如果是 编辑器模式 并且从 Assets 加载资源 一直返回True
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static bool IsLoadedAssetBundle(string projectName, string bundleName)
        {
#if UNITY_EDITOR
            if (AssetBundleManager.GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                // 如果是 编辑器模式 并且从 Assets 加载资源 一直返回True
                return true;
            }
#endif
            return assetBundles.ContainsKey(projectName) && assetBundles[projectName].ContainsKey(bundleName);
        }

        /// <summary>
        /// 加载某个模块所有的AssetBundle 异步
        /// </summary>
        /// <param name="projectName">项目名</param>
        /// <param name="onProgressChange">进度改变的事件</param>
        /// <param name="onFinsh">加载完成的事件</param>
        public static LoadAllAssetBundlesRequest LoadAllAssetBundles(string projectName)
        {
            string key = string.Format("LoadAllAssetBundles:{0}", projectName);

            return ExecuteOnlyOnceAtATime<LoadAllAssetBundlesRequest>(key, () =>
            {
                LoadAllAssetBundlesRequest request = new LoadAllAssetBundlesRequest();
                CoroutineStarter.Start(request.LoadAllAssetBundles(projectName));
                return request;
            });
        }

        /// <summary>
        /// 卸载某个AssetBundle
        /// </summary>
        /// <param name="projectName">项目名</param>
        /// <param name="bundleName">AssetBundle名</param>
        public static void UnLoadAssetBundle(string projectName, string bundleName)
        {
            if (IsLoadedAssetBundle(projectName, bundleName))
            {
                assetBundles[projectName][bundleName].Unload(true);
                assetBundles[projectName].Remove(bundleName);
            }
        }
        // 卸载 某个 Project 的所有AssetBundle
        public static void UnLoadAllAssetBundles(string projectName, bool unloadAllLoadedObjects = true)
        {
            if (assetBundles.ContainsKey(projectName))
            {
                foreach (var bundleName in assetBundles[projectName].Keys)
                {
                    assetBundles[projectName][bundleName].Unload(unloadAllLoadedObjects);
                }
                assetBundles[projectName].Clear();
            }
        }

#endregion

        #region 加载资源

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName">没有后缀</param>
        /// <returns></returns>
        public static T LoadAsset<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object
        {

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName) ) {
                return null;
            }

#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                return AssetDatabase.LoadAssetAtPath<T>(bundle != null ? bundle.GetAssetPathByFileName(assetName) : string.Empty);
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            AssetBundle assetBundle = LoadAssetBundle(projectName, bundle_name);
            return assetBundle != null ? assetBundle.LoadAsset<T>(assetName) : null;
        }
        // 加载资源
        public static UnityEngine.Object LoadAsset(string projectName, string bundleName, string assetName, Type type)
        {

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName))
            {
                return null;
            }

#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                //string path = bundle.GetAssetPathByFileName(assetName);
                //Debug.LogFormat("path:{0} type:{1}",path,type.Name);
                return AssetDatabase.LoadAssetAtPath(bundle != null ? bundle.GetAssetPathByFileName(assetName) : string.Empty, type);
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            AssetBundle assetBundle = LoadAssetBundle(projectName, bundle_name);
            return assetBundle != null ? assetBundle.LoadAsset(assetName, type) : null;
        }

        // 异步加载资源 
        public static LoadAssetRequest LoadAssetAsync<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object
        {
            LoadAssetRequest request = new LoadAssetRequest();
            CoroutineStarter.Start(request.LoadAssetAsync<T>(projectName, bundleName, assetName));
            return request;
        }
        public static LoadAssetRequest LoadAssetAsync(string projectName, string bundleName, string assetName, Type type)
        {
            LoadAssetRequest request = new LoadAssetRequest();
            CoroutineStarter.Start(request.LoadAssetAsync(projectName, bundleName, assetName, type));
            return request;
        }

        // 加载子资源 
        public static T[] LoadAssetWithSubAssets<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName) ) {
                return null;
            }
#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                return bundle != null ? ArrayCast<T>(AssetDatabase.LoadAllAssetsAtPath(bundle.GetAssetPathByFileName(assetName))) : null;
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            AssetBundle assetBundle = LoadAssetBundle(projectName, bundle_name);
            return assetBundle != null ? assetBundle.LoadAssetWithSubAssets<T>(assetName) : null;
        }
        public static UnityEngine.Object[] LoadAssetWithSubAssets(string projectName, string bundleName, string assetName, Type type)
        {

            if ( string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName) ) {
                return null;
            }

#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                return bundle != null ? ArrayCast(AssetDatabase.LoadAllAssetsAtPath(bundle.GetAssetPathByFileName(assetName)), type) : null;
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            AssetBundle assetBundle = LoadAssetBundle(projectName, bundle_name);
            return assetBundle != null ? assetBundle.LoadAssetWithSubAssets(assetName, type) : null;
        }

        /// <summary>
        /// 异步加载子资源
        /// </summary>
        public static LoadAssetsRequest LoadAssetWithSubAssetsAsync<T>(string projectName, string bundleName, string assetName) where T : UnityEngine.Object
        {
            LoadAssetsRequest request = new LoadAssetsRequest();
            CoroutineStarter.Start(request.LoadAssetWithSubAssetsAsync<T>(projectName, bundleName, assetName));
            return request;
        }
        /// <summary>
        /// 异步加载子资源
        /// </summary>
        public static LoadAssetsRequest LoadAssetWithSubAssetsAsync(string projectName, string bundleName, string assetName, Type type)
        {
            LoadAssetsRequest request = new LoadAssetsRequest();
            CoroutineStarter.Start(request.LoadAssetWithSubAssetsAsync(projectName, bundleName, assetName, type));
            return request;
        }

        /// <summary>
        /// 加载所有资源
        /// </summary>
        /// <returns></returns>
        public static UnityEngine.Object[] LoadAllAssets(string projectName, string bundleName)
        {
            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) )
            {
                return null;
            }
#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                return LoadAssetsFromAssets(bundle.GetAllAssetPaths());
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            AssetBundle assetBundle = LoadAssetBundle(projectName, bundle_name);
            return assetBundle != null ? assetBundle.LoadAllAssets() : null;
        }
        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="bundleName"></param>
        public static LoadAssetsRequest LoadAllAssetsAsync(string projectName, string bundleName)
        {
            LoadAssetsRequest request = new LoadAssetsRequest();
            CoroutineStarter.Start(request.LoadAllAssetsAsync(projectName, bundleName));
            return request;
        }

        /// <summary>
        /// 加载某个类型所有资源
        /// </summary>
        /// <returns></returns>
        public static T[] LoadAllAssets<T>(string projectName, string bundleName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) )
            {
                return null;
            }
#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                return ArrayCast<T>(LoadAssetsFromAssets(bundle.GetAllAssetPaths()));
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            AssetBundle assetBundle = LoadAssetBundle(projectName, bundle_name);
            return assetBundle != null ? assetBundle.LoadAllAssets<T>() : null;
        }

        /// <summary>
        /// 加载某个类型所有资源
        /// </summary>
        /// <returns></returns>
        public static UnityEngine.Object[] LoadAllAssets(string projectName, string bundleName, Type type)
        {

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) )
            {
                return null;
            }

#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                return ArrayCast(LoadAssetsFromAssets(bundle.GetAllAssetPaths()), type);
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            AssetBundle assetBundle = LoadAssetBundle(projectName, bundle_name);
            return assetBundle != null ? assetBundle.LoadAllAssets(type) : null;
        }

        /// <summary>
        /// 异步加载某个类型的所有资源
        /// </summary>
        public static LoadAssetsRequest LoadAllAssetsAsync<T>(string projectName, string bundleName) where T : UnityEngine.Object
        {
            LoadAssetsRequest request = new LoadAssetsRequest();
            CoroutineStarter.Start(request.LoadAllAssetsAsync<T>(projectName, bundleName));
            return request;
        }
        /// <summary>
        /// 异步加载某个类型的所有资源
        /// </summary>
        public static LoadAssetsRequest LoadAllAssetsAsync(string projectName, string bundleName, Type type)
        {
            LoadAssetsRequest request = new LoadAssetsRequest();
            CoroutineStarter.Start(request.LoadAllAssetsAsync(projectName, bundleName, type));
            return request;
        }

        public static void LoadScene(string projectName, string bundleName, string sceneName, LoadSceneMode mode) {

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) )
            {
                return;
            }

#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                //bundle.GetAssetPathByFileName(assetName);
                EditorSceneManager.LoadSceneInPlayMode(bundle.GetAssetPathByFileName(sceneName),new LoadSceneParameters() { loadSceneMode = mode });
                return;
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            // 加载场景所在 AB
            LoadAssetBundle(projectName, bundle_name);
            // 加载场景
            SceneManager.LoadScene(sceneName);
        }

        public static AsyncOperation LoadSceneAsync(string projectName, string bundleName, string sceneName, LoadSceneMode mode) {

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(sceneName) )
            {
                return null;
            }

#if UNITY_EDITOR
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
                //bundle.GetAssetPathByFileName(assetName);
                return EditorSceneManager.LoadSceneAsyncInPlayMode(bundle.GetAssetPathByFileName(sceneName), new LoadSceneParameters() { loadSceneMode = mode });
            }
#endif
            string bundle_name = string.Format("{0}_{1}", projectName, bundleName);
            LoadAssetBundle(projectName, bundle_name);
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }

#endregion

        #region 方法

        /// <summary>
        /// 判断是否有内置资源
        /// </summary>
        /// <param name="projectName">资源模块名</param>
        /// <returns></returns>
        public static IsHaveBuiltInResRequest IsHaveBuiltInRes(string projectName )
        {
            IsHaveBuiltInResRequest request = new IsHaveBuiltInResRequest();
            CoroutineStarter.Start(request.IsHaveBuiltInRes(projectName));
            return request;
        }

        /// <summary>
        /// 判断本地是否有资源
        /// </summary>
        /// <param name="projectName">资源项目名称</param>
        /// <returns></returns>
        public static bool IsHaveResOnLocal(string projectName)
        {
            return File.Exists(string.Format("{0}/{1}", XFABTools.DataPath(projectName), XFABConst.project_build_info));
        }

        /// <summary>
        /// 设置获取 版本的接口 
        /// </summary>
        /// <param name="projectVersion"></param>
        public static void SetGetProjectVersion<T>() where T : IGetProjectVersion
        {
            getProjectVersion = typeof(T);
        }

        public static void SetGetProjectVersion(Type type) {

            if( XFABTools.IsImpInterface(type,typeof(IGetProjectVersion)) == false)
                throw new Exception("类型必须实现 IGetProjectVersion 接口!");
            getProjectVersion = type;
        }

        [Obsolete("改方法已经过时,将会在未来的版本中移除,请使用方法 SetGetProjectVersion<T>() 或者 SetGetProjectVersion(Type type)代替!", false)]
        public static void SetGetProjectVersion(IGetProjectVersion getProjectVersion) {
            if(getProjectVersion == null)
                throw new Exception("getProjectVersion is null!");
            SetGetProjectVersion(getProjectVersion.GetType());
        }

        /// <summary>
        /// 设置服务端文件路径接口 注:此方法要在 AssetBundleManager 初始化完成之后调用 才会生效!
        /// </summary>
        /// <param name="serverFilePath"></param>
        public static void SetServerFilePath(IServerFilePath serverFilePath) {
            ServerFilePath = serverFilePath;
        }

        /// <summary>
        /// 获取某个项目的依赖项目
        /// </summary>
        /// <param name="projectName">项目名</param>
        public static GetProjectDependenciesRequest GetProjectDependencies(string projectName)
        {
            GetProjectDependenciesRequest request = new GetProjectDependenciesRequest();
            CoroutineStarter.Start(request.GetProjectDependencies(projectName));
            return request;
        }

        /// <summary>
        /// 获取AssetBundle的路径
        /// </summary>
        /// <param name="projectName">项目名</param>
        /// <param name="bundleName">AssetBundle名称 需要加后缀!</param>
        /// <returns></returns>
        public static string GetAssetBundlePath(string projectName, string bundleName, string suffix = "")
        {
            return XFABTools.LocalResPath(projectName, string.Format("{0}{1}", bundleName, suffix));
        }
        /// <summary>
        /// 获取AssetBundle的依赖
        /// </summary>
        /// <param name="projectName">项目名</param>
        /// <param name="bundleName">bundle名 不需要加后缀</param>
        /// <returns></returns>
        public static string[] GetAssetBundleDependences(string projectName, string bundleName)
        {
            string dependenceName = XFABTools.GetCurrentPlatformName();

            if (!dependenceProjectName.Equals(projectName) && dependenceBundle != null)
            {
                dependenceBundle.Unload(true);
                dependenceBundle = null;
            }

            if (dependenceBundle == null)
            {
                string path = GetAssetBundlePath(projectName, dependenceName);
                dependenceBundle = LoadAssetBundle(path);
                dependenceProjectName = projectName;
            }

            // 加载依赖的AssetBundle
            AssetBundleManifest manifest = dependenceBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            string[] dependences = manifest.GetAllDependencies(string.Format("{0}{1}", bundleName, GetAssetBundleSuffix(projectName)));
            for (int i = 0; i < dependences.Length; i++)
            {
                dependences[i] = Path.GetFileNameWithoutExtension(dependences[i]);
            }
            return dependences;
        }

        internal static T ExecuteOnlyOnceAtATime<T>(string key, AsyncOperationDelegate execute) where T : CustomYieldInstruction
        {
            if (async_cache.ContainsKey(key))
            {
                return async_cache[key] as T;
            }

            CustomYieldInstruction operation = execute?.Invoke();

            async_cache.Add(key, operation);

            CoroutineStarter.Start(WaitCustomYieldInstruction(operation,()=> {
                async_cache.Remove(key);
            }));

            return operation as T;
        }

        private static IEnumerator WaitCustomYieldInstruction(CustomYieldInstruction operation,Action onFinsh) {
            yield return operation;
            onFinsh?.Invoke();
        }

        /// <summary>
        /// 获取某一个资源模块 在本地的所有资源大小
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static long GetProjectCacheSize(string projectName) {
            return FileTools.GetDirectorySize(XFABTools.DataPath(projectName));
        }

        /// <summary>
        /// 清空某一个模块在本地的所有资源文件
        /// </summary>
        /// <param name="projectName"></param>
        public static void ClearProjectCache(string projectName) {
            FileTools.DeleteDirectory(XFABTools.DataPath(projectName));
        }


#if UNITY_EDITOR
        /// <summary>
        /// 获取 Asset 目录下资源路径 
        /// </summary>
        /// <param name="projectName">项目名</param>
        /// <param name="bundleName">bundle名</param>
        /// <param name="assetName">资源名不含后缀</param>
        /// <returns></returns>
        private static string GetAssetPathFromAsset(string projectName, string bundleName, string assetName)
        {

            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName) ) {
                Debug.LogErrorFormat("projectName:{0} bundleName:{1} assetName:{2}",projectName,bundleName,assetName);
                return null;
            }

            XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
            return bundle != null ? bundle.GetAssetPathByFileName(assetName) : null;
        }

        /// <summary>
        /// 获取某个bundle下 所有资源的路径
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private static string[] GetAllAssetPathFromAsset(string projectName, string bundleName)
        {
            XFABAssetBundle bundle = GetXFABAssetBundle(projectName, bundleName);
            return bundle != null ? bundle.GetAllAssetPaths() : null;
        }


        /// <summary>
        /// 获取 XFABAssetBundle( 编辑器数据 )
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private static XFABAssetBundle GetXFABAssetBundle(string projectName, string bundleName)
        {
            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(bundleName) ) {
                return null;
            }
            if (GetProfile(projectName).loadMode == LoadMode.Assets)
            {
                XFABProject project = XFABProjectManager.Instance.GetProject(projectName);
                if (project != null)
                {
                    XFABAssetBundle bundle = project.GetAssetBundle(bundleName);
                    if (bundle != null)
                    {
                        return bundle;
                    }
                    else
                    {
                        Debug.LogErrorFormat("LoadAsset error ! XFABAssetBundle:{0} is null!", bundleName);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("LoadAsset error ! 未查询到项目配置 :{0}! 如果是从AssetBundle文件中加载资源,请把 LoadMode 设置为 AssetBundle模式!", projectName);
                }

                return null;
            }
            return null;
        }


        /// <summary>
        /// 从Asset目录加载资源
        /// </summary>
        /// <param name="asset_paths"></param>
        /// <returns></returns>
        private static UnityEngine.Object[] LoadAssetsFromAssets(string[] asset_paths)
        {
            UnityEngine.Object[] objects = new UnityEngine.Object[asset_paths.Length];
            if (asset_paths != null)
            {
                for (int i = 0; i < asset_paths.Length; i++)
                {
                    objects[i] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(asset_paths[i]);
                }
            }
            return objects;
        }

#endif
        /// <summary>
        /// 数组类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static T[] ArrayCast<T>(UnityEngine.Object[] objects) where T : UnityEngine.Object
        {
            if (objects == null)
            {
                throw new Exception("objects is null!");
            }
            List<T> list = new List<T>(objects.Length);
            for (int i = 0; i < objects.Length; i++)
            {
                T t = objects[i] as T;
                if (t != null)
                {
                    list.Add(t);
                }
            }
            return list.ToArray();
        }

        private static UnityEngine.Object[] ArrayCast(UnityEngine.Object[] objects, Type type)
        {
            if (objects == null)
            {
                throw new Exception("objects is null!");
            }
            List<UnityEngine.Object> list = new List<UnityEngine.Object>(objects.Length);

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].GetType() == type)
                {
                    list.Add(objects[i]);
                }
            }

            return list.ToArray();
        }

        public static Profile GetProfile(string projectName = "") {

            if (string.IsNullOrEmpty(projectName))
            {
                return profiles.Where(x => x.name.Equals("Default")).FirstOrDefault();
            }
            else
            {
                foreach (var item in profiles)
                {
                    if (item.ProjectName.Equals(projectName))
                    {
                        return item;
                    }
                }
            }

            return profiles.Where(x => x.name.Equals("Default")).FirstOrDefault();
        }

        private static IGetProjectVersion CreateProjectVersionInstance()
        {
            if (getProjectVersion == null) return null;

            if (XFABTools.IsBaseByClass(getProjectVersion, typeof(MonoBehaviour)))
            {
                GameObject obj = new GameObject(getProjectVersion.FullName);
                GameObject.DontDestroyOnLoad(obj);
                return obj.AddComponent(getProjectVersion) as IGetProjectVersion;
            }
            else
                return Activator.CreateInstance(getProjectVersion) as IGetProjectVersion;
        }

        internal static void ReleaseProjectVersionInstance(IGetProjectVersion version)
        {
            if (version != null && XFABTools.IsBaseByClass(version.GetType(), typeof(MonoBehaviour)))
            {
                MonoBehaviour mono = version as MonoBehaviour;
                GameObject.Destroy(mono.gameObject);
            }
        }

        #endregion
    }

}

