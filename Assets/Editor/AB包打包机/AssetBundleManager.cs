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
    private string outpath = "";
    private bool an;
    private bool ios;
    private bool file;
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导出ab包"))
        {
            BuildAllAssetBundles();
        }
        GUILayout.EndHorizontal();

        if ( string.IsNullOrEmpty( outpath))
        {
            outpath = Application.dataPath;
        }
        
        EditorGUI.LabelField(new Rect(0,25,100,20),"当前选择的AB包:");
        if (selectPkg!=null)
        {
            EditorGUI.LabelField(new Rect(100,25,80,20),selectPkg.pkgName);
        }
        EditorGUI.LabelField(new Rect(180,25,60,20),"输出路径:");
        outpath= EditorGUI.TextField(new Rect(250,25,260,20),outpath);
        file=GUI.Toggle(new Rect(510,25,100,20),file,"简化文件名称" );
        
        an=GUI.Toggle(new Rect(610,25,80,20),an,"安卓打包");
        ios=GUI.Toggle(new Rect(690,25,80,20),ios,"IOS打包");
        
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
                Save();
            }

            for (int i = 0; i < pkgName.Count; i++)
            {
                pkgName[i].pkgName =
                    GUI.TextField(new Rect(box1.x + 0, box1.y + 20 + i * 25, 115, 20), pkgName[i].pkgName);
                if (GUI.Button(new Rect(box1.x + 150, box1.y + 20 + i * 25, 35, 20), "删除"))
                {
                    pkgName.RemoveAt(i);
                    Save();
                }

                if (GUI.Button(new Rect(box1.x + 115, box1.y + 20 + i * 25, 35, 20), "选择"))
                {
                    selectPkg = pkgName[i];
                }
            }

            GUI.EndScrollView();
        }
        if (selectPkg != null)
        {
            {
                GUI.Box(box3, "");
                var pkgs = selectPkg.AllAB;
                pos3 = GUI.BeginScrollView(
                    box3,
                    pos3,
                    new Rect(220, 55, WindowWidth - 220,
                        pkgs.Count * 25 > WindowHeight - 55 ? pkgs.Count * 25 : WindowHeight - 55),
                    false,
                    true);
                DragAndDropTool.CreationDragAndDropArea(
                    box3,
                    DragAction, selectPkg.AllAB.Count ==0 ? "将文件拖到到这里":"");

                for (int i = 0; i < pkgs.Count; i++)
                {
                    if (!File.Exists(pkgs[i].path))
                    {
                        selectPkg.ReMoveAsset(pkgs[i].path);
                        Save();
                    }
                    GUI.Box(new Rect(box3.x, box3.y + i * 25, 20, 20), pkgs[i].GeIcon());
                    if (file)
                    {
                        EditorGUI.LabelField(new Rect(box3.x + 20, box3.y + i * 25, box3.width - 70, 20), pkgs[i].GetFileName());
                    }
                    else
                    {
                        EditorGUI.LabelField(new Rect(box3.x + 20, box3.y + i * 25, box3.width - 70, 20), pkgs[i].path);
                    }
                    if (GUI.Button(new Rect(box3.x +box3.width - 50, box3.y + i * 25, 35, 20), "删除"))
                    {
                        selectPkg.ReMoveAsset(pkgs[i].path);
                        Save();
                    }
                }

                GUI.EndScrollView();
            }
        }
    }

    private Pkg selectPkg;
    private List<Pkg> pkgName=new List<Pkg>();
    private Rect box1 => new Rect(0, 55, 200, WindowHeight - 55);
    private Rect box3 => new Rect(220, 55, WindowWidth - 220, WindowHeight - 55);
    private Vector2 pos=new Vector2();
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

        if (pkgName==null)
        {
            pkgName = new List<Pkg>();
        }
    }

    private void Save()
    {
        string path = Application.dataPath+"/Editor/AB包打包机/data.json";
        FileTools.WriteFile(path,SerializeTools.ObjToString(pkgName));
    }

    private void DragAction(string[] strs)
    {
        foreach (var item in strs)
        {
            if (Directory.Exists(item))
            {
                //对象是个文件夹
            }
            else
            {
                selectPkg.AddABAsset(item);
            }
        }
        Save();
    }
    

    private void BuildAllAssetBundles()
    {

        foreach (var item in pkgName)
        {
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            if (item.GetAllAbStringArray().Length==0||string.IsNullOrEmpty( item.pkgName))
            {
                Debug.LogWarning("跳过"+item.pkgName+" 的打包");
                continue;
            }
            assetBundleBuild.assetNames =item.GetAllAbStringArray() ;
            assetBundleBuild.assetBundleName = item.pkgName;
            if ( string.IsNullOrEmpty(outpath))
            {
                outpath = Application.dataPath;
            }
        
            if (an)
            {
                if (!Directory.Exists(  outpath + "/Android"))
                {
                    Directory.CreateDirectory(outpath + "/Android");
                }
            
                BuildPipeline.BuildAssetBundles(outpath + "/Android", new []{assetBundleBuild},
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
}


