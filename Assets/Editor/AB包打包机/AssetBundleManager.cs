using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.Tools;
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


       {
           GUI.Box(box1, "");
           pos = GUI.BeginScrollView(
               box1,
               pos,
               new Rect(0, 55, 185, pkgName.Count * 25 > WindowHeight - 55 ? pkgName.Count * 25 : WindowHeight - 55),
               false,
               true);
           if (GUI.Button(new Rect(box1.x + 0, box1.y, 185, 20), "添加AB包"))
           {
               pkgName.Add(new Pkg());
           }
           for (int i = 0; i < pkgName.Count; i++)
           {
               pkgName[i].pkgName=GUI.TextField(new Rect(box1.x + 0, box1.y + 20 + i * 25, 115, 20), pkgName[i].pkgName);
               if (GUI.Button(new Rect(box1.x + 150, box1.y + 20 + i * 25, 35, 20), "删除"))
               {
                   pkgName.RemoveAt(i);
               }
               if (GUI.Button(new Rect(box1.x + 115, box1.y + 20 + i * 25, 35, 20), "选择"))
               {
                   selectPkg=pkgName[i];
               }
           }

           GUI.EndScrollView();
       }
       if (selectPkg!=null)
       {
           {
               GUI.Box(box2, "");
               pos2 = GUI.BeginScrollView(
                   box2,
                   pos2,
                   new Rect(210, 55, 185,
                       pkgDir.Count * 25 > WindowHeight - 55 ? pkgDir.Count * 25 : WindowHeight - 55),
                   false,
                   true);
               DragAndDropTool.CreationDragAndDropArea(
                   box2,
                   DragAction, "将文件夹拖到到这里");
               for (int i = 0; i < selectPkg.dirs.Count; i++)
               {
                   GUI.Box(new Rect(box2.x, box2.y + i * 25, 20, 20),
                       (Texture2D) AssetDatabase.GetCachedIcon(selectPkg.dirs[i]));
                   GUI.TextField(new Rect(box2.x + 20, box2.y + i * 25, box2.width - 30, 20), selectPkg.dirs[i]);
               }

               GUI.EndScrollView();
           }

           {
               GUI.Box(box3, "");
               pos3 = GUI.BeginScrollView(
                   box3,
                   pos3,
                   new Rect(420, 55, WindowWidth - 420,
                       files.Count * 25 > WindowHeight - 55 ? files.Count * 25 : WindowHeight - 55),
                   false,
                   true);
               DragAndDropTool.CreationDragAndDropArea(
                   box3,
                   DragAction2, "将文件拖到到这里");
               var pkgs = selectPkg.all();
               for (int i = 0; i < pkgs.Count; i++)
               {
                   GUI.Box(new Rect(box3.x, box3.y + i * 25, 20, 20), pkgs[i].GeIcon);
                   GUI.TextField(new Rect(box3.x + 20, box3.y + i * 25, box3.width - 30, 20), pkgs[i].path);
               }

               GUI.EndScrollView();
           }
       }
    }

    private Pkg selectPkg;
    private List<Pkg> pkgName=new List<Pkg>();
    private List<string> pkgDir = new List<string>();
    private List<string> files = new List<string>();
    private Rect box1 => new Rect(0, 55, 200, WindowHeight - 55);
    private Rect box2 => new Rect(210, 55, 200, WindowHeight-55);
    private Rect box3 => new Rect(420, 55, WindowWidth - 420, WindowHeight - 55);
    private Vector2 pos;
    private Vector2 pos2;
    private Vector2 pos3;

    private void OnEnable()
    {
        string path = Application.dataPath+"/Editor/AB包打包机/data.json";
        try
        {        
            pkgName=SerializeTools.StringToObj<List<Pkg>>(FileTools.ReadFile(path));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void Save()
    {
        string path = Application.dataPath+"/Editor/AB包打包机/data.json";
        FileTools.WriteFile(path,SerializeTools.ObjToString(pkgName));
    }

    private void DragAction2(string[] strs)
    {
        foreach (var item in strs)
        {
            selectPkg.addABAsset(item);
        }

        Save();
    }

    private void DragAction(string[] strs)
    {
        
        foreach (var item in strs)
        {
            selectPkg.addDir(item);
        }

        Save();
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


