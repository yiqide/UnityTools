#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XFABManager
{


    [System.Serializable]
    public enum XFBundleType
    {

        File,       // 文件 
        Directory  // 目录 档期这个文件夹下所有的文件

    }

    [System.Serializable]
    public class FileInfo {
        public string guid;
        
        public string displayName => Path.GetFileNameWithoutExtension(AssetPath);

        public XFBundleType type;

        private long size;
        private System.IO.FileInfo fileInfo;
        public string AssetPath {
            get {
                return AssetDatabase.GUIDToAssetPath(guid);
            }
        }

        public long Size {
            get {

                if (type == XFBundleType.Directory)
                {

                    size = 0;
                    // 获取目录下所有文件 
                    string[] guids = AssetDatabase.FindAssets("", new string[] { AssetPath });

                    for (int i = 0; i < guids.Length; i++)
                    {
                        if (!AssetBundleTools.IsValidAssetBundleFile(AssetDatabase.GUIDToAssetPath(guids[i]))) { continue; }

                        FileInfo f = new FileInfo(guids[i]);
                        size += f.Size;
                    }


                }
                else if(type == XFBundleType.File)
                {
                    if (fileInfo == null)
                    {
                        fileInfo = new System.IO.FileInfo(AssetPath);
                        
                    }
                    if (fileInfo.Exists)
                        size = fileInfo.Length;
                    else { 
                        size = 0;
                    }

                }


                return size;
            }
        }
        public string SizeString {

            get {
                if (Size == 0)
                    return "--";
                return EditorUtility.FormatBytes(size);
            }
        }

        public bool Exists
        {
            get
            {
                if (string.IsNullOrEmpty(AssetPath))
                {
                    return false;
                }

                if (type == XFBundleType.File)
                {

                    return File.Exists(AssetPath);
                }
                else
                {
                    return Directory.Exists(AssetPath);
                }
            }
        }

        public FileInfo() { }

        public FileInfo(string guid) {
            this.guid = guid;
            //this.displayName = Path.GetFileNameWithoutExtension( AssetPath );
            //DirectoryInfo directoryInfo = new DirectoryInfo(AssetPath);
            this.type = AssetDatabase.IsValidFolder(AssetPath) ? XFBundleType.Directory : XFBundleType.File;
        }

    }

    [System.Serializable]
    public class XFABAssetBundle
    {

        public string bundle_name;
        public List<FileInfo> files;
        public string projectName;
        private long size;

        public long Size {

            get {
                size = 0;
                for (int i = 0; i < files.Count; i++)
                {
                    size += files[i].Size;
                }

                return size;
            }
        }


        public string SizeString
        {
            get
            {
                if (Size == 0)
                    return "--";
                return EditorUtility.FormatBytes(size);
            }
        }

        private XFABProject project;

        private XFABProject Project {
            get {
                if (project == null) {
                    project = XFABProjectManager.Instance.GetProject(projectName);
                }
                return project;
            }
        }

        public XFABAssetBundle(string projectName) {
            files = new List<FileInfo>();
            this.projectName = projectName;
        }

        public XFABAssetBundle(string bundleName, string projectName) {
            bundle_name = bundleName;
            files = new List<FileInfo>();
            this.projectName = projectName;
        }

        public void AddFile(string assetPath) {
            //string asset_path = AssetDatabase.GUIDToAssetPath(guid);
            if ( string.IsNullOrEmpty(assetPath) )
            {
                Debug.Log("文件不存在!");
                return;
            }

            // 判断这个文件是不是存在别的AssetBundle中 ( 一个文件 只能存在一个AssetBundle中 )
            RemoveOtherBundleFile(assetPath);

            // 如果是文件夹 判断这个文件夹下所有的文件
            if (AssetDatabase.IsValidFolder(assetPath))
            {

                string[] guids = AssetDatabase.FindAssets("", new string[] { assetPath });

                for (int j = 0; j < guids.Length; j++)
                {
                    RemoveOtherBundleFile(AssetDatabase.GUIDToAssetPath(guids[j]));
                }
            }


            FileInfo fileInfo = new FileInfo(AssetDatabase.AssetPathToGUID(assetPath));
            AddFile(fileInfo);
        }

        // 移除其他 bundle 中的文件
        private void RemoveOtherBundleFile(string asset_path)
        {
            if ( Project == null ) {
                Debug.LogErrorFormat("{0} is null! ",projectName);
                return;
            }
            XFABAssetBundle currentFileBundle = Project.GetBundleByContainFile(asset_path);
            if (currentFileBundle != null && currentFileBundle != this)
            {
                currentFileBundle.RemoveFile(AssetDatabase.AssetPathToGUID(asset_path));
            }
        }

        // 添加文件 
        private void AddFile(FileInfo fileInfo) {

            // 判断是不是已经存在的
            if ( IsContainFile(fileInfo.guid) ) {

                Debug.LogErrorFormat( "文件{0}已经存在,不能重复添加!",fileInfo.AssetPath);
                return;
            }

            if ( IsDuplicateNames(fileInfo.displayName) ) 
            {
                Debug.LogErrorFormat("名称{0}重复,不能添加同名文件!不区分大小写!", fileInfo.displayName);
                return;
            }

            Debug.Log(fileInfo.type);

            if (fileInfo.type == XFBundleType.Directory)
            {
                // 判断当前这个文件夹下的文件 是否 存在 这个Bundle中 ， 如果存在 需要移除
                string[] assets = AssetDatabase.FindAssets("",new string[] { fileInfo.AssetPath });

                for (int i = 0; i < assets.Length; i++)
                {
                    if ( IsContainFile(assets[i]) ) {
                        RemoveFile(assets[i]);
                    }
                }
            }
            else if ( fileInfo.type == XFBundleType.File ) {
                // 判断是否在 当前 Bundle 的文件夹中 
                if ( IsContainFileInFolder(fileInfo.guid) ) {
                    Debug.LogWarning(string.Format("文件{0}已经存在文件夹中,不需要添加!", fileInfo.AssetPath));
                    return;
                }
            }

            files.Add(fileInfo);
        }
        // 移除文件
        private void RemoveFile(FileInfo file) {

            if ( file != null ) {
                files.Remove(file);
            }

        }

        public void RemoveFile(string guid) {
            RemoveFile(GetFileInfo(guid));
        }

        public FileInfo GetFileInfo(string guid) {

            for (int i = 0; i < files.Count; i++)
            {
                if (guid.Equals(files[i].guid))
                {
                    return files[i];
                }
            }
            return null;
        }

        // 判断是不是已经包含文件
        public bool IsContainFile(string guid) {

            for (int i = 0; i < files.Count; i++)
            {
                if ( guid.Equals( files[i].guid ) ) {
                    return true;
                }
            }

            return false;
        }

        // 判断是不是有同名文件
        public bool IsDuplicateNames(string fileName) {

            for (int i = 0; i < files.Count; i++)
            {
                if ( fileName.ToLower().Equals(files[i].displayName.ToLower()) ) {
                    return true;
                }
            }

            return false;
        }

        // 判断 这个Bundle中的文件夹 是否包含某个文件
        public bool IsContainFileInFolder(string guid) {

            List<string> folders = new List<string>();

            for (int i = 0; i < files.Count; i++)
            {
                if ( files[i].type == XFBundleType.Directory ) {

                    // 获取这个文件夹下所有的资源 
                    folders.Add(files[i].AssetPath);
                }
            }

            if (folders.Count > 0)
            {
                string[] assets = AssetDatabase.FindAssets("", folders.ToArray());
                for (int i = 0; i < assets.Length; i++)
                {
                    if (guid.Equals(assets[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<FileInfo> GetFileInfos() {

            UpdateFileInfos();
            return files;
        }

        public string[] GetFilePaths() {

            List<string> paths = new List<string>();

            List<FileInfo> fileInfos = GetFileInfos();
            for (int i = 0; i < fileInfos.Count; i++)
            {
                paths.Add(fileInfos[i].AssetPath);
            }

            return paths.ToArray();
        }

        // 更新文件列表
        public void UpdateFileInfos() {

            for (int i = 0; i < files.Count; i++)
            {
                // 判断文件是否存在 如果不在就移除
                if (!files[i].Exists)
                {
                    files.Remove(files[i]);
                }
            }

        }

        public string GetAssetPathByFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) {
                return null;
            }

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].type == XFBundleType.File)
                {
                    if (files[i].displayName.Equals(fileName))
                    {
                        return files[i].AssetPath;
                    }
                }
                else if (files[i].type == XFBundleType.Directory)
                {
                    // 获取到目录下面的所有文件 来比较
                    string[] assets = AssetDatabase.FindAssets(fileName, new string[] { files[i].AssetPath });

                    foreach (var asset in assets)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                        if ( Path.HasExtension(assetPath) && Path.GetFileNameWithoutExtension(assetPath).Equals(fileName) )
                        {
                            return assetPath;
                        }
                    }

                }
            }
            return null;
        }


        public string[] GetAllAssetPaths() {

            //string[] paths = new string[files.Count];

            List<string> paths = new List<string>(files.Count);

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].type == XFBundleType.File)
                {
                    if (!paths.Contains(files[i].AssetPath)) { 
                        paths.Add(files[i].AssetPath);
                    }
                }
                else if(files[i].type == XFBundleType.Directory) {
                    // 获取到这个目录下面的所有文件

                    string[] guids = AssetDatabase.FindAssets("", new string[] { files[i].AssetPath });

                    for (int j = 0; j < guids.Length; j++)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guids[j]);
                        if (!AssetBundleTools.IsValidAssetBundleFile(assetPath)) { continue; }

                        if (!paths.Contains( assetPath )) { 
                            paths.Add(assetPath);
                        }
                    }


                }
            }


            return paths.ToArray();

        }

    }


}

#endif