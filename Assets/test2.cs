using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class test2: EditorWindow
{
    
    [MenuItem("菜单/打开窗口")]
    private static void ShowWindow()
    {
        GetWindow<test2>().Show();
    }
    private void OnGUI()
    {
        
    }

    [MenuItem("Assets/创建AB包")]
    static void BuildAllAssetBundles()
    {
        string an = "/Users/ddw/Downloads/AB/an";
        string ios = "/Users/ddw/Downloads/AB/ios";
        if (!Directory.Exists(an))
        {
            Directory.CreateDirectory(an);
        }if (!Directory.Exists(ios))
        {
            Directory.CreateDirectory(ios);
        }
        
        BuildPipeline.BuildAssetBundles(an,
            BuildAssetBundleOptions.None,
            BuildTarget.Android);/*
        BuildPipeline.BuildAssetBundles(ios,
            BuildAssetBundleOptions.None,
            BuildTarget.iOS);
            */
        
    }
}


