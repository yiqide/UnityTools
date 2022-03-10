using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



public class ABAsset
{
    public string path;
    public Texture2D GeIcon => (Texture2D)AssetDatabase.GetCachedIcon(path);
}
