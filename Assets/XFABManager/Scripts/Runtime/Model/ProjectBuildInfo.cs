using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFABManager
{

    [Serializable]
    public class BundleInfo {
        public string bundleName;
        public long bundleSize;
        public string md5;

        public BundleInfo(string bundleName,long bundleSize,string md5) {
            this.bundleName = bundleName;
            this.bundleSize = bundleSize;
            this.md5 = md5;
        }

    }

    [Serializable]
    public class ProjectBuildInfo
    {
        /// <summary>
        /// 项目名
        /// </summary>
        public string projectName;
        /// <summary>
        /// 显示名
        /// </summary>
        public string displayName;
        /// <summary>
        /// 依赖的项目 (所有)
        /// </summary>
        public string[] dependenceProject;
        /// <summary>
        /// AssetBundle 的信息
        /// </summary>
        public BundleInfo[] bundleInfos;
        /// <summary>
        /// 后缀
        /// </summary>
        public string suffix;
        /// <summary>
        /// 版本
        /// </summary>
        public string version;
        /// <summary>
        /// 更新的内容
        /// </summary>
        public string update_message;

    }
}


