using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



public class ABAsset
{
    public string path;
    public Texture2D GeIcon => (Texture2D)AssetDatabase.GetCachedIcon(path);

    public ABAsset(string path)
    {
        this.path = path;
    }
}

public class Pkg
{
    public string pkgName="";
    public List<string> dirs=new List<string>();
    
    private List<ABAsset> otherAllAB = new List<ABAsset>();

    public void addDir(string path)
    {
        if (Directory.Exists(path)&&!dirs.Contains(path))
        {
            dirs.Add(path);
        }
    }

    public void addABAsset(string path)
    {
        foreach (var item in  all())
        {
            if (item.path==path)
            {
                return;
            }
        }
        otherAllAB.Add(new ABAsset(path));
    }

    public List<ABAsset> all()
    {
        var  all =new List<ABAsset>();
        all.AddRange(otherAllAB);
        foreach (var item in dirs)
        {
           var files= Directory.GetFiles(item);
           foreach (var VARIABLE in files)
           {
               if (VARIABLE.Contains(".meta"))
               {
                   continue;
               }
               all.Add(new ABAsset(VARIABLE));
           }
        }

        return all;
    }

}