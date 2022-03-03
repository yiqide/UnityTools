using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace XFABManager
{

    /// <summary>
    /// 内置资源类型
    /// </summary>
    //public enum BuildInResType
    //{
    //    /// <summary>
    //    /// 文件集合
    //    /// </summary>
    //    Files,
    //    /// <summary>
    //    /// 压缩文件
    //    /// </summary>
    //    Zip
    //}

    public class IsHaveBuiltInResRequest : CustomAsyncOperation<IsHaveBuiltInResRequest>
    {

        //public BuildInResType buildInResType { get; private set; }
        public bool isHave { get; private set; }

        internal IEnumerator IsHaveBuiltInRes(string projectName)
        {
            yield return null;
            //string zipPath = string.Format("{0}{1}.zip", XFABTools.BuildInDataPath(projectName), projectName);
            string project_build_info = string.Format("{0}{1}", XFABTools.BuildInDataPath(projectName), XFABConst.project_build_info);

#if UNITY_ANDROID && !UNITY_EDITOR
            //bool isHaveZip = false;
            //bool isHaveFiles = false;

            //UnityWebRequest requestZip = UnityWebRequest.Get(zipPath);
            //yield return requestZip.SendWebRequest();
            //isHaveZip = string.IsNullOrEmpty(requestZip.error);

            UnityWebRequest requestFiles = UnityWebRequest.Get(project_build_info);
            yield return requestFiles.SendWebRequest();
            isHave = string.IsNullOrEmpty(requestFiles.error);

#else
            //bool isHaveZip = File.Exists(zipPath);
            isHave = File.Exists(project_build_info);
#endif

            //isHave = isHaveFiles;
            //buildInResType = isHaveZip ? BuildInResType.Zip : BuildInResType.Files;
            Completed();
        }

    }


}


