using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
//using UnityEditor.Build.Reporting;
using UnityEngine;

namespace XFABManager
{
    public class PlayerBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder {
            get {
                return 0;
            }
        }
        public void OnPreprocessBuild(BuildReport report)
        {
            //Debug.Log("准备打包"+target + "path:" + path);
            // 创建目录
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            string profile_setting = string.Format("{0}/{1}", Application.streamingAssetsPath, XFABConst.profile_setting);
            // 判断是否有文件
            if (!File.Exists(profile_setting))
            {
                File.Create(profile_setting).Close();
            }

            File.WriteAllText(profile_setting, JsonUtility.ToJson(new ProfileConfig( XFABManagerSettings.Settings.CurrentProfiles )));
            AssetDatabase.Refresh();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            //Debug.Log("打包完成!"+target + "path:" + pathToBuiltProject);

            string profile_setting = string.Format("{0}/{1}", Application.streamingAssetsPath, XFABConst.profile_setting);
            // 判断是否有文件
            if (File.Exists(profile_setting))
            {
                File.Delete(profile_setting);
            }
            AssetDatabase.Refresh();
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
            // 判断文件夹是不是空的 如果是空就删掉
            if (directoryInfo.Exists)
            {
                if (directoryInfo.GetDirectories().Length == 0 && directoryInfo.GetFiles().Length == 0)
                {
                    // 删除文件夹
                    Directory.Delete(Application.streamingAssetsPath);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}


