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

    private string OutPath
    {
        get { return outpath; }
        set
        {
            if (string.Equals( outpath,value))
            {
                return;
            }
            outpath = value;
            Save();
        }
    }
    private bool an;
    private bool ios;
    private bool stadia;
    private bool noTarget;
    private bool file;
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导出ab包"))
        {
            BuildAllAssetBundles();
        }
        GUILayout.EndHorizontal();

        if ( string.IsNullOrEmpty( OutPath))
        {
            OutPath = Application.dataPath;
        }
        
        EditorGUI.LabelField(new Rect(0,25,100,20),"当前选择的AB包:");
        if (selectPkg!=null)
        {
            EditorGUI.LabelField(new Rect(100,25,80,20),selectPkg.pkgName);
        }
        EditorGUI.LabelField(new Rect(180,25,60,20),"输出路径:");
        OutPath= EditorGUI.TextField(new Rect(250,25,260,20),OutPath);
        file=GUI.Toggle(new Rect(510,25,100,20),file,"简化文件名称" );
        
        an=GUI.Toggle(new Rect(610,25,80,20),an,"安卓打包");
        ios=GUI.Toggle(new Rect(690,25,80,20),ios,"IOS打包");
        stadia=GUI.Toggle(new Rect(780,25,80,20),stadia,"Stadia打包");
        noTarget=GUI.Toggle(new Rect(860,25,100,20),noTarget,"NoTarget打包");
        
        {
            GUI.Box(box1, "");
            pos = GUI.BeginScrollView(
                box1,
                pos,
                new Rect(0, 55, 185, pkgName.Count * 25 > WindowHeight - 55 ? pkgName.Count * 25 : WindowHeight - 55),
                false,
                true);
            if (GUI.Button(new Rect(box1.x + 0, box1.y, box1.width-20, 20), "添加AB包"))
            {
                pkgName.Add(new Pkg());
                Save();
            }

            for (int i = 0; i < pkgName.Count; i++)
            {
                pkgName[i].pkgName =
                    GUI.TextField(new Rect(box1.x + 0, box1.y + 20 + i * 25, 250, 20), pkgName[i].pkgName);
                
                if (GUI.Button(new Rect(box1.x + 250, box1.y + 20 + i * 25, 35, 20), "选择"))
                {
                    selectPkg = pkgName[i];
                }
                
                if (GUI.Button(new Rect(box1.x + 285, box1.y + 20 + i * 25, 35, 20), "删除"))
                {
                    pkgName.RemoveAt(i);
                    Save();
                }

                if (!是否打包.ContainsKey(pkgName[i]))
                {
                    是否打包.Add(pkgName[i],false);
                }
                是否打包[pkgName[i]]=GUI.Toggle(new Rect(box1.x + 320, box1.y + 20 + i * 25, 20, 20), 是否打包[pkgName[i]],"" );

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
                    new Rect(box3.x, box3.y, box3.width,
                        pkgs.Count * 25 > WindowHeight - 55 ? pkgs.Count * 25 : WindowHeight - 55),
                    false,
                    true);
                DragAndDropTool.CreationDragAndDropArea(
                    new Rect(box3.x, box3.y,  box3.width,
                        pkgs.Count * 25 > WindowHeight - 55 ? pkgs.Count * 25 : WindowHeight - 55),
                    DragAction, selectPkg.AllAB.Count ==0 ? "将文件拖到到这里":"");

                for (int i = 0; i < pkgs.Count; i++)
                {
                    if (!File.Exists(pkgs[i].path))
                    {
                        Debug.LogWarning("找不到："+pkgs[i].path+"\n 已从列表中移除!");
                        selectPkg.ReMoveAsset(pkgs[i].path);
                        i--;
                        Save();
                        continue;
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
    private LocalFile _localFile;
    private List<Pkg> pkgName=new List<Pkg>();
    private Dictionary<Pkg, bool> 是否打包 = new Dictionary<Pkg, bool>();
    private Rect box1 => new Rect(0, 55, 350, WindowHeight - 55);
    private Rect box3 => new Rect(370, 55, WindowWidth - 360, WindowHeight - 55);
    private Vector2 pos=new Vector2();
    private Vector2 pos2;
    private Vector2 pos3;

    private void OnEnable()
    {
        string path = Application.dataPath+"/Editor/AB包打包机/data.json";
        try
        {        
            _localFile=SerializeTools.StringToObj<LocalFile>(FileTools.ReadFile(path));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        if (_localFile==null)
        {
            _localFile = new LocalFile();
        }
        pkgName = _localFile.pkgName;
        OutPath = _localFile.path;
    }

    private void Save()
    {
        _localFile.path = OutPath;
        string path = Application.dataPath+"/Editor/AB包打包机/data.json";
        FileTools.WriteFile(path,SerializeTools.ObjToString(_localFile));
    }

    private void DragAction(string[] strs)
    {
        foreach (var item in strs)
        {
            if (Directory.Exists(item))
            {
                //对象是个文件夹
                var list= FileTools.GetAllFile(item);
                for (int i = 0; i < list.Count; i++)
                {
                    if (Path.GetFileName( list[i]).Contains(".meta")||Path.GetFileName( list[i]).Contains(".cs"))
                    {
                        list.RemoveAt(i);
                        i--;
                    }
                }

                foreach (var itemFile in list)
                {
                    selectPkg.AddABAsset(itemFile);
                }
            }
            else
            {
                if (!Path.GetFileName(item).Contains(".cs"))
                {
                    selectPkg.AddABAsset(item);
                }
            }
        }
        Save();
    }
    
    private void BuildAllAssetBundles()
    {

        foreach (var item in pkgName)
        {
            if (!是否打包[item])
            {
                continue;
            }

            if (string.IsNullOrEmpty( item.pkgName))
            {
                Debug.LogWarning("这个ab包的名称是空的，名称不能为空，打包时已跳过");
                continue;
            }
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            if (item.GetAllAbStringArray().Length==0||string.IsNullOrEmpty( item.pkgName))
            {
                Debug.LogWarning("跳过"+item.pkgName+" 的打包");
                continue;
            }
            assetBundleBuild.assetNames =item.GetAllAbStringArray() ;
            assetBundleBuild.assetBundleName = item.pkgName;
            if ( string.IsNullOrEmpty(OutPath))
            {
                OutPath = Application.dataPath;
            }
        
            if (an)
            {
                if (!Directory.Exists(  OutPath + "/Android"))
                {
                    Directory.CreateDirectory(OutPath + "/Android");
                }
            
                BuildPipeline.BuildAssetBundles(OutPath + "/Android", new []{assetBundleBuild},
                    BuildAssetBundleOptions.None,BuildTarget.Android);
            }
            if (ios)
            {
                if (!Directory.Exists(  OutPath + "/IOS"))
                {
                    Directory.CreateDirectory(OutPath + "/IOS");
                }
                BuildPipeline.BuildAssetBundles(OutPath + "/IOS", new []{assetBundleBuild},
                    BuildAssetBundleOptions.None,BuildTarget.iOS);
            }
            if (stadia)
            {
                if (!Directory.Exists(  OutPath + "/Stadia"))
                {
                    Directory.CreateDirectory(OutPath + "/Stadia");
                }
            
                BuildPipeline.BuildAssetBundles(OutPath + "/Android", new []{assetBundleBuild},
                    BuildAssetBundleOptions.None,BuildTarget.Stadia);
            }
            if (noTarget)
            {
                if (!Directory.Exists(  OutPath + "/NoTarget"))
                {
                    Directory.CreateDirectory(OutPath + "/NoTarget");
                }
            
                BuildPipeline.BuildAssetBundles(OutPath + "/Android", new []{assetBundleBuild},
                    BuildAssetBundleOptions.None,BuildTarget.NoTarget);
            }
        }
        
    }
}

[Serializable]
public class LocalFile
{
    public List<Pkg> pkgName;
    public string path;

    public LocalFile()
    {
        pkgName = new List<Pkg>();
        path = "";
    }
}

