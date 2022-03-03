using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace XFABManager
{
    public class ProjectBuild
    {

        private static List<AssetBundleBuild> bundles = new List<AssetBundleBuild>();
 

        public static void Build(XFABProject project, BuildTarget buildTarget) {

            // 刷新资源
            AssetDatabase.Refresh();

            // 对 project 内容做一个保存
            project.Save();

            BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;
            for (int i = 0; i < project.buildAssetBundleOptions.Count; i++)
            {
                if (project.buildAssetBundleOptions[i].isOn)
                {
                    buildAssetBundleOptions |= project.buildAssetBundleOptions[i].option;
                }
            }

            bundles.Clear();

            for (int i = 0; i < project.assetBundles.Count; i++)
            {
                string[] asset_paths = project.assetBundles[i].GetFilePaths();
                if (asset_paths.Length > 0)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = string.Format("{0}_{1}{2}", project.name.ToLower(),project.assetBundles[i].bundle_name,project.suffix);
                    build.assetNames = asset_paths;
                    bundles.Add(build);
                }
            }

            if (bundles.Count == 0)
            {
                Debug.LogErrorFormat(" 项目: {0} 文件列表为空,请添加需要打包的文件!",project.name);
                return;
            }

            // 如果目录存在 并且清空的话 就删除输出目录
            if (project.isClearFolders)
            {
                DeleteFolders(project.out_path(buildTarget));
            }

            // 清空数据目录
            DeleteFolders(XFABTools.DataPath(project.name));

            // 如果输出目录不存在 创建目录
            if (!Directory.Exists(project.out_path(buildTarget)))
            {
                Directory.CreateDirectory(project.out_path(buildTarget));
            }

            var buildManifest = BuildPipeline.BuildAssetBundles(project.out_path(buildTarget), bundles.ToArray(), buildAssetBundleOptions,buildTarget);

            if (buildManifest == null)
            {
                Debug.LogError("Error in build");
                return;
            }
            AssetDatabase.Refresh();

            ProjectBuildInfo buildInfo = new ProjectBuildInfo();
            buildInfo.bundleInfos = BuildBundlesInfo(project,buildTarget);
            buildInfo.dependenceProject = project.GetAllDependencies();
            buildInfo.displayName = project.displayName;
            buildInfo.projectName = project.name;
            buildInfo.suffix = project.suffix;
            buildInfo.update_message = project.update_message;
            buildInfo.version = project.version;

            string project_build_info = string.Format("{0}/{1}", project.out_path(buildTarget), XFABConst.project_build_info);
            if (!File.Exists(project_build_info)) File.Create(project_build_info).Close();

            File.WriteAllText(project_build_info, JsonUtility.ToJson(buildInfo));

            if (project.isCompressedIntoZip)
            {
                CompressedIntoZip(project,buildTarget);
            }

            if (project.isCopyToStreamingAssets)
            {
                CopyToStreamingAssets(project, buildTarget);
            }

            Debug.Log(string.Format("打包完成! module:{0} target:{1} version:{2}", project.name, buildTarget, project.version));

            EditorApplication.delayCall += ()=> {
                OnBuildComplete(project, buildTarget);
            };


            EditorUtility.RevealInFinder(project.out_path(buildTarget));
        }

        // 删除文件夹
        public static bool DeleteFolders(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (System.IO.IOException)
                {
                    Debug.LogError( string.Format("操作无法完成\n{0}\n因为其中的文件或文件夹 已在另一个程序中打开!\n请关闭后重试!", path));
                    return false;
                }
            }
            return true;
        }

        // 构建Bundle文件列表
        private static BundleInfo[] BuildBundlesInfo(XFABProject project,BuildTarget buildTarget)
        {
            //string file_list = string.Format("{0}/{1}", output_path, XFABConst.files);
            //if (File.Exists(file_list)) File.Delete(file_list);

            bundles.Add(new AssetBundleBuild() { assetBundleName = buildTarget.ToString() });

            //List<BundleInfo> bundleInfos = new List<BundleInfo>( bundles.Count );

            BundleInfo[] bundleInfos = new BundleInfo[bundles.Count];
            // 构建bundle的
            for (int i = 0; i < bundles.Count; i++)
            {
                string file = string.Format("{0}/{1}", project.out_path(buildTarget), bundles[i].assetBundleName);
                bundleInfos[i] = BuildBundleFileInfo(file, bundles[i].assetBundleName);
            }

            return bundleInfos;
        }

        public static BundleInfo BuildBundleFileInfo(string filePath, string fileName)
        {

            string md5 = XFABTools.md5file(filePath);
            long length = 0;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            if (fileInfo.Exists)
                length = fileInfo.Length;
            else
            {
                Debug.LogErrorFormat("文件:{0} 不存在!", filePath);
            }
            return new BundleInfo(fileName, length, md5);
        }

        // 复制资源 到 StreamingAssets
        public static bool CopyToStreamingAssets( XFABProject project,BuildTarget buildTarget )
        {

            string targetPath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, project.name, buildTarget.ToString());

            // 清空目标文件夹的内容
            if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }

            Directory.CreateDirectory(targetPath);
            // 判断有没有压缩文件 如果有压缩 复制压缩文件 如果没有 复制 assetbundles

            string zipFile = string.Format("{0}/{1}.zip", project.out_path(buildTarget), project.name);

            bool isSuccess = false;

            //if (File.Exists(zipFile))
            //{
            //    // 复制压缩文件
            //    File.Copy(zipFile, string.Format("{0}/{1}.zip", targetPath, project.name), true);
            //    isSuccess = true;
            //}
            //else
            //{
            // 复制assetbundle文件
            isSuccess = FileTools.CopyDirectory(project.out_path(buildTarget), targetPath, (fileName, progress) =>
            {
                EditorUtility.DisplayProgressBar("复制到StreamingAssets", string.Format("正在复制文件:{0}", fileName), progress);
            }, new string[] { ".zip" });
            EditorUtility.ClearProgressBar();
            //}
            AssetDatabase.Refresh();

            return isSuccess;
        }

        // 压缩资源
        public static bool CompressedIntoZip(XFABProject project,BuildTarget buildTarget)
        {
            string zipPath = string.Format("{0}/{1}.zip", project.out_path(buildTarget), project.name);
            return ZipTools.CreateZipFile(project.out_path(buildTarget), zipPath);
        }

        // 
        private static void OnBuildComplete(XFABProject project,BuildTarget buildTarget) {

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                //Debug.Log(item.FullName);
                if ( !item.FullName.StartsWith("Assembly-CSharp") ) {
                    continue;
                }
                foreach (var type in item.GetTypes())
                {
                    foreach (var t in type.GetInterfaces())
                    {
                        if ( t == typeof(IOnBuildComplete)) {
                            IOnBuildComplete onBuild = Activator.CreateInstance(type) as IOnBuildComplete;
                            onBuild.OnBuildComplete(project.name, project.out_path(buildTarget), buildTarget);
                        }
                    }
                }
            }

        }

    }

}

