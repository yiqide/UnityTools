using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class test2: BaseEditorWindow<test2>
{
    
    [MenuItem("菜单/打开窗口")]
    private static void ShowWindow()
    {
        GetWindow<test2>().Show();
    }

    private List<string> str = new List<string>();
    private string pakName="";
    private string outpath = "";
    private bool an;
    private bool ios;
    private void OnGUI()
    {
        /*
        DragAndDropTool.CreationDragAndDropArea(new Rect(0,0, WindowWidth ,200),DragAction,"将文件拖到到这里");
        GUILayout.Space(100);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导出ab包"))
        {
            BuildAllAssetBundles();
        }
        if (GUILayout.Button("清空"))
        {
            str.Clear();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("包名:");
        pakName=GUILayout.TextField(pakName);
        GUILayout.Label("输出路径:");
        outpath=GUILayout.TextField(outpath);
        an=GUILayout.Toggle(an,"安卓打包");
        ios=GUILayout.Toggle(ios,"IOS打包");
        GUILayout.EndHorizontal();
        
        for (int i = 0; i < str.Count; i++)
        {
            GUILayout.TextField(str[i]);
        }
        GUILayout.BeginHorizontal();
        */
        pos=GUI.BeginScrollView(
            new Rect(0,50,WindowWidth,200),
            pos,
            new Rect(0,0,WindowWidth,400),
            true,
            true);
        GUI.Label(new Rect(0,0,WindowWidth,30),"你好1");
        GUI.Label(new Rect(0,35,WindowWidth,30),"你好2");

        GUI.Label(new Rect(0,70,WindowWidth,30),"你好");

        GUI.Label(new Rect(0,105,WindowWidth,30),"你好");

        GUI.Label(new Rect(0,140,WindowWidth,30),"你好");
        GUI.Label(new Rect(0,175,WindowWidth,30),"你好");
        GUI.Label(new Rect(0,210,WindowWidth,30),"你好");

        GUI.Label(new Rect(0,245,WindowWidth,30),"你好");

        GUI.Label(new Rect(0,280,WindowWidth,30),"你好");

        GUI.Label(new Rect(0,315,WindowWidth,30),"你好900");
        GUI.Box(new Rect(0,350,WindowWidth,30),"大家好" );
        GUI.EndScrollView();
       
    }

    private Vector2 pos;

    private void DragAction(string[] strs)
    {
        str.AddRange(strs);
    }

    private void BuildAllAssetBundles()
    {

        AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
        assetBundleBuild.assetNames = str.ToArray();
        assetBundleBuild.assetBundleName = pakName;
        if ( string.IsNullOrEmpty(outpath))
        {
            outpath = Application.dataPath;
        }
        
        if (an)
        {
            if (!Directory.Exists(  outpath + "/安卓"))
            {
                Directory.CreateDirectory(outpath + "/安卓");
            }
            
            BuildPipeline.BuildAssetBundles(outpath + "/安卓", new []{assetBundleBuild},
                BuildAssetBundleOptions.None,BuildTarget.Android);
        }
        if (ios)
        {
            if (!Directory.Exists(  outpath + "/IOS"))
            {
                Directory.CreateDirectory(outpath + "/IOS");
            }
            BuildPipeline.BuildAssetBundles(outpath + "/IOS", new []{assetBundleBuild},
                BuildAssetBundleOptions.None,BuildTarget.iOS);
        }
    }
}


