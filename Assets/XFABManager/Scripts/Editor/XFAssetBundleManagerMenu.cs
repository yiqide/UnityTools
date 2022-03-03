using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XFABManager
{


    public class XFAssetBundleManagerMenu : MonoBehaviour
    {
        [MenuItem("XFABManager/Projects", false, 1)]
        static void XFAssetBundleManager()
        {

            XFAssetBundleManagerProjects window = EditorWindow.GetWindow<XFAssetBundleManagerProjects>("XFABManager");
            window.Show();

        }
             
        [MenuItem("XFABManager/About", false, 2000)]
        static void Help()
        {
            Rect rect = new Rect(0, 0, 550, 350);
            XFAssetBundleManagerHelp window = EditorWindow.GetWindowWithRect<XFAssetBundleManagerHelp>(rect, true, "About XFABManager");
            window.Show();

        }

        //[MenuItem("XFABManager/Test Window", false, 3000)]
        //static void Test()
        //{
        //    XFAssetBundleProjectMain window = EditorWindow.GetWindow<XFAssetBundleProjectMain>("Project");
        //    window.Show();
        //}

    }
}
