using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[Serializable]
public class ABAsset
{
    public string path="";
    public Texture2D GeIcon()
    {
        return (Texture2D)AssetDatabase.GetCachedIcon(path);
    }

    public ABAsset(string path)
    {
        this.path = path;
    }

    public string GetFileName()
    {
        string str;
        str = Path.GetFileName(path);
        return str;
    }
}
[Serializable]
public class Pkg
{
    public string pkgName="";

    public List<ABAsset> AllAB = new List<ABAsset>();

    public string[] GetAllAbStringArray()
    {
        List<string> list = new List<string>();
        foreach (var item in AllAB)
        {
            list.Add(item.path);
        }
        return list.ToArray();
    }

    public void AddABAsset(string path)
    {
        foreach (var item in  AllAB)
        {
            if (item.path==path)
            {
                return;
            }
        }
        AllAB.Add(new ABAsset(path));
    }

    public void ReMoveAsset(string path)
    {
        ABAsset asset=null;
        foreach (var item in  AllAB)
        {
            if (item.path==path)
            {
                asset = item;
                break;
            }
        }

        if (asset!=null)
        {
            AllAB.Remove(asset);
        }
    }
}