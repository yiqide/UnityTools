
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace XFABManager
{

    [System.Serializable]
    public class XFABProject : UnityEngine.ScriptableObject
    {

        // 项目名称
        [HideInInspector]
        public new string name;     // 要唯一 不能重复
        
        public string displayName;  // 显示名称 随便

        // 这个项目里面有哪些 AB 包 
        [HideInInspector]
        public List<XFABAssetBundle> assetBundles;

        private string _out_path = null;

        public string out_path(BuildTarget buildTarget)
        {
            if (string.IsNullOrEmpty(_out_path))
            {
                _out_path = string.Format("{0}AssetBundles", Application.dataPath.Substring(0, Application.dataPath.Length - 6));
                //_out_path = Application.dataPath.Replace("Assets", "AssetBundles");
            }
            return string.Format("{0}/{1}/{2}/{3}", _out_path, name, version, buildTarget);

        }

        [HideInInspector]
        public List<string> dependenceProject;
        [HideInInspector]
        public string suffix;       // ab 包后缀名 默认.unity3d
        [HideInInspector]
        public string version;      // 当前这个AB的版本

        [HideInInspector]
        public bool isClearFolders;         // 是否清空 AssetBundles
        [HideInInspector]
        public bool isCopyToStreamingAssets;// 是否复制到 StreamingAssets
        [HideInInspector]
        public bool isCompressedIntoZip;    // 是否压缩成 zip 

        [HideInInspector]
        public string update_message;// 更新信息

        [HideInInspector]
        public List<BuildOptionToggleData> buildAssetBundleOptions;


        public string Title
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(name);
                if (!string.IsNullOrEmpty(displayName))
                {
                    builder.Append("/").Append(displayName);
                }
                return builder.ToString();
            }
        }

        public XFABProject()
        {
            assetBundles = new List<XFABAssetBundle>();
            dependenceProject = new List<string>();
            InitBuildOptions();
        }

        public void InitBuildOptions() {

            buildAssetBundleOptions = new List<BuildOptionToggleData>();
            buildAssetBundleOptions.Add(new BuildOptionToggleData("None", "默认构建AssetBundle的方式.(使用LZMA算法压缩,此算法压缩包小,但是加载时间长(仅第一次,再次加载会很快!).)", true, BuildAssetBundleOptions.None));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("UncompressedAssetBundle", "不压缩数据,包大,但是加载很快", false, BuildAssetBundleOptions.UncompressedAssetBundle));
            //buildAssetBundleOption.Add(new ToggleData("CollectDependencies", "", true, BuildAssetBundleOptions.CollectDependencies));
            //buildAssetBundleOption.Add(new ToggleData("CompleteAssets", "", true, BuildAssetBundleOptions.CompleteAssets));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("DisableWriteTypeTree", "不会在AssetBundle中包含类型信息", false, BuildAssetBundleOptions.DisableWriteTypeTree));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("DeterministicAssetBundle", "使用存储在Asset Bundle中的对象的id的哈希构建Asset Bundle.", false, BuildAssetBundleOptions.DeterministicAssetBundle));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("ForceRebuildAssetBundle", "强制重建Asset Bundles", false, BuildAssetBundleOptions.ForceRebuildAssetBundle));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("IgnoreTypeTreeChanges", "执行增量构建检查时忽略类型树更改", false, BuildAssetBundleOptions.IgnoreTypeTreeChanges));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("AppendHashToAssetBundleName", "将哈希附加到assetBundle名称", false, BuildAssetBundleOptions.AppendHashToAssetBundleName));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("ChunkBasedCompression", "使用LZ4算法压缩,压缩率没有LZMA高,但加载稍快,中规中矩", false, BuildAssetBundleOptions.ChunkBasedCompression));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("StrictMode", "如果在其中报告任何错误，则不允许构建成功", false, BuildAssetBundleOptions.StrictMode));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("DryRunBuild", "不实际构建它们", false, BuildAssetBundleOptions.DryRunBuild));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("DisableLoadAssetByFileName", "通过文件名禁用Asset Bundle的加载", false, BuildAssetBundleOptions.DisableLoadAssetByFileName));
            buildAssetBundleOptions.Add(new BuildOptionToggleData("DisableLoadAssetByFileNameWithExtension", "通过带扩展名的文件名禁用AssetBundle 的加载.", false, BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension));

#if UNITY_2019_1_OR_NEWER
            buildAssetBundleOptions.Add(new BuildOptionToggleData("AssetBundleStripUnityVersion", "在构建时删除文件中的Unity版本号.", false, BuildAssetBundleOptions.AssetBundleStripUnityVersion));
#endif
        }

        // 是否依赖于某个项目
        public bool IsDependenceProject(string name  ) {


            // 当前这个项目是否依赖这个项目 
            if (dependenceProject.Contains(name)) {
                return true;
            }

            // 当前这个项目 依赖的项目 是否依赖这个项目 
            for (int i = 0; i < dependenceProject.Count; i++)
            {
                XFABProject project = XFABProjectManager.Instance.GetProject(dependenceProject[i]);
                if (project != null &&  project.IsDependenceProject(name)) {
                    return true;
                }
            }
            return false;
        }

        public void Save() {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            //Debug.Log("保存 XFABProject!");

            //AssetDatabase.Refresh();
        }

        public bool AddAssetBundle(XFABAssetBundle bundle ) {

            // 判断名称是否重复 
            if ( IsContainAssetBundleName( bundle.bundle_name ) ) {
                Debug.LogError("添加失败! AssetBundle 名称重复!");
                return false;
            }

            // 判断当前这个AssetBundle中的文件是否再其他的AssetBundle中存在 如果存在需要移除
            List<FileInfo> files = bundle.GetFileInfos();
            for (int i = 0; i < files.Count; i++)
            {
                for (int j = 0; j < assetBundles.Count; j++)
                {
                    if (assetBundles[j].IsContainFile(files[i].guid)) {
                        assetBundles[j].RemoveFile(files[i].guid);
                    }
                }
            }


            assetBundles.Add(bundle);

            Save();

            return true;

        }

        public bool IsContainAssetBundleName(string name) {


            for (int i = 0; i < assetBundles.Count; i++)
            {
                if ( name.Equals( assetBundles[i].bundle_name) ) {
                    return true;
                }
            }

            return false;

        }

        // 判断这个Project 是不是包含某个文件
        public bool IsContainFile(string asset_path) {

            for (int i = 0; i < assetBundles.Count; i++)
            {
                if ( assetBundles[i] != null ) {

                    if (assetBundles[i].IsContainFile(AssetDatabase.AssetPathToGUID(asset_path))) {
                        return true;
                    }
                }
            }


            return false;

        }

        public bool RenameAssetBundle( string newName,string originName ) {
            XFABAssetBundle assetBundle = GetAssetBundle(originName);

            if ( IsContainAssetBundleName(newName) ) {

                Debug.LogError(" 新名称已经存在!不能重复! ");
                return false;
            }

            if ( assetBundle != null ) 
            {
                assetBundle.bundle_name = newName;
                Save();
            }
            return true;
        }

        public XFABAssetBundle GetAssetBundle(string bundle_name) {

            if ( string.IsNullOrEmpty(bundle_name) ) {

                //Debug.Log( string.Format( " bundle_name:{0} 异常! ",bundle_name ) );
                return null;
            }

            for (int i = 0; i < assetBundles.Count; i++)
            {
                if ( bundle_name.Equals( assetBundles[i].bundle_name ) ) {
                    return assetBundles[i];
                }
            }
            return null;
        }

        public XFABAssetBundle GetAssetBundle(int nameHashCode) {

            for (int i = 0; i < assetBundles.Count; i++)
            {
                if (nameHashCode == assetBundles[i].bundle_name.GetHashCode())
                {
                    return assetBundles[i];
                }
            }
            return null;
        }

        public void RemoveAssetBundle(XFABAssetBundle bundle) {

            assetBundles.Remove(bundle);
        }

        // 删除Bundle
        public void RemoveAssetBundle(string bundle_name) {

            if ( IsContainAssetBundleName(bundle_name) )
            {
                RemoveAssetBundle( GetAssetBundle(bundle_name) );
                Save();
            }

        }

        public void RemoveAssetBundle(int nameHashCode) {

            XFABAssetBundle bundle = GetAssetBundle(nameHashCode);
            if ( bundle != null )
            {
                RemoveAssetBundle(bundle);
                Save();
            }
            

        }

        // 获取Bundle 通过 它包含的文件
        public XFABAssetBundle GetBundleByContainFile(string asset_path) {
            for (int i = 0; i < assetBundles.Count; i++)
            {
                if (assetBundles[i].IsContainFile( AssetDatabase.AssetPathToGUID( asset_path ) )) {
                    return assetBundles[i];
                }
            }

            return null;
        }

        // 获取依赖的AssetBundle
        public List<XFABAssetBundle> GetDependenciesBundles(XFABAssetBundle bundle) {

            List<XFABAssetBundle> bundles = new List<XFABAssetBundle>();

            // 获取bundle中所有的文件 
            string[] asset_paths = AssetDatabase.GetDependencies(bundle.GetFilePaths(), true);

            // 遍历所有的AssetBundle
            for (int i = 0; i < assetBundles.Count; i++)
            {
                if (assetBundles[i] != bundle) {

                    for (int j = 0; j < asset_paths.Length; j++)
                    {
                        if (assetBundles[i].IsContainFile(AssetDatabase.AssetPathToGUID(asset_paths[j]))) {
                            bundles.Add(assetBundles[i]);
                            break;
                        }
                    }
                
                }
            }

            return bundles;

        }


        /// <summary>
        /// 获取依赖的所有项目
        /// </summary>
        /// <returns></returns>
        public string[] GetAllDependencies() {

            List<string> dependence = new List<string>();
            dependence.AddRange(dependenceProject);

            for (int i = 0; i < dependenceProject.Count; i++)
            {
                XFABProject project = XFABProjectManager.Instance.GetProject(dependenceProject[i]);
                if (project==null) { continue; }
                dependence.AddRange(project.GetAllDependencies());
            }

            return dependence.ToArray();
        }


    }

}

#endif