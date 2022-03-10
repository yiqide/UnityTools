using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleManager: BaseEditorWindow<AssetBundleManager>
{
    
    [MenuItem("Tools/AssetBundle打包机")]
    private static void ShowWindow()
    {
        GetWindow<AssetBundleManager>().titleContent.text="AssetBundle打包机";
        GetWindow<AssetBundleManager>().Show();
    }

    private List<string> str = new List<string>();
    private string pakName="";
    private string outpath = "";
    private bool an;
    private bool ios;
    private void OnGUI()
    {

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
       // GUILayout.Label("包名:");
        //pakName=GUILayout.TextField(pakName);
        GUILayout.Label("输出路径:");
        outpath=GUILayout.TextField(outpath);
        an=GUILayout.Toggle(an,"安卓打包");
        ios=GUILayout.Toggle(ios,"IOS打包");
        GUILayout.EndHorizontal();
       // DragAndDropTool.CreationDragAndDropArea(new Rect(0,50, WindowWidth ,50),DragAction,"将文件拖到到这里");
       /*
        pos=GUI.BeginScrollView(
            new Rect(0,100,WindowWidth,WindowHeight-100),
            pos,
            new Rect(0,0,WindowWidth,200+35*str.Count),
            true,
            true);

        int y = 0;
        for (int i = 0; i < str.Count; i++)
        {
            Texture2D icon = AssetDatabase.GetCachedIcon(str[i]) as Texture2D;
            GUI.Box(new Rect(0,y,20,20),icon );
            GUI.TextField(new Rect(25,y,WindowWidth-25,20),str[i]);
            y += 25;
        }
        GUI.EndScrollView();*/
        
        

        GUI.Box(new Rect(0, 55, 200, WindowHeight-55), "你哈");
        pos = GUI.BeginScrollView(
            new Rect(0,55, 200 ,WindowHeight-55),
            pos,  
            new Rect(0,55,185,pkgName.Count*25 >WindowHeight-55?pkgName.Count*25:WindowHeight-55),
            false,
            true);
        
        GUI.EndScrollView();
        
        GUI.Box(new Rect(210, 55, 200, WindowHeight-55), "你哈");
        pos2 = GUI.BeginScrollView(
            new Rect(210,55, 200 ,WindowHeight-55),
            pos2,  
            new Rect(210,55,185,pkgDir.Count*25 >WindowHeight-55?pkgDir.Count*25:WindowHeight-55),
            false,
            true);
        
        GUI.EndScrollView();
        
        
        GUI.Box(new Rect(420, 55, WindowWidth-420, WindowHeight-55), "你哈");
        pos3 = GUI.BeginScrollView(
            new Rect(420,55, WindowWidth-420 ,WindowHeight-55),
            pos3,  
            new Rect(420,55,WindowWidth-420,files.Count*25 >WindowHeight-55?files.Count*25:WindowHeight-55),
            false,
            true);
        
        GUI.EndScrollView();
    }

    private List<string> pkgName=new List<string>();
    private List<string> pkgDir = new List<string>();
    private List<string> files = new List<string>();
    private Vector2 pos;
    private Vector2 pos2;
    private Vector2 pos3;

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


