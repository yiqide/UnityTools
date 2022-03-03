using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XFABManager {
    public class GetProjectDependenciesRequest : CustomAsyncOperation<GetProjectDependenciesRequest>
    {
        //private AssetBundleManager bundleManager;

        private List<string> dependon_list;

        public string[] dependencies
        {
            get {
                return dependon_list.ToArray();
            }
        }

        public GetProjectDependenciesRequest() {
            dependon_list = new List<string>();
        }

        internal IEnumerator GetProjectDependencies(string projectName ) {
            
            if (string.IsNullOrEmpty(projectName))
            {
                yield return new WaitForEndOfFrame();
                Completed(string.Format("项目名不能为空!"));
                yield break;
            }
            //string[] _dependencies = null;

            string project_build_info_content = null;

            if (AssetBundleManager.GetProfile(projectName).updateModel == UpdateMode.UPDATE)
            {
                GetProjectVersionRequest requestVersion = AssetBundleManager.GetProjectVersion(projectName);
                // 获取版本
                yield return requestVersion;
 
                if (!string.IsNullOrEmpty(requestVersion.error))
                {
                    error = string.Format("请求{0}版本出错!:{1}", projectName, requestVersion.error);
                    isCompleted = true;
                    yield break;
                }
                // 获取依赖项目
                GetFileFromServerRequest requestDepend = AssetBundleManager.GetFileFromServer(projectName, requestVersion.version, XFABConst.project_build_info);
                yield return requestDepend;
                if (string.IsNullOrEmpty(requestDepend.error))
                {
                    project_build_info_content = requestDepend.text;
                }
                else
                {
                    error = string.Format("获取{0}依赖出错:{1} url:{2} ", projectName, requestDepend.error,requestDepend.request_url);
                    isCompleted = true;
                    yield break;
                }
            }
            else
            {
                // 从本地文件读取
                string dependonFile = XFABTools.LocalResPath(projectName, XFABConst.project_build_info);
                if (File.Exists(dependonFile))
                {
                    project_build_info_content = File.ReadAllText(dependonFile);
                }
            }

            if (!string.IsNullOrEmpty(project_build_info_content))
            {
                ProjectBuildInfo projectBuildInfo = JsonUtility.FromJson<ProjectBuildInfo>(project_build_info_content);
                dependon_list.AddRange( projectBuildInfo.dependenceProject );
            }

            isCompleted = true;
        }
        

    }
}


