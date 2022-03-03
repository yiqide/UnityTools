using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XFABManager
{

#if UNITY_EDITOR

    public class AssetBundleTools
    {

        public static bool IsValidAssetBundleFile(string asset_path)
        {
            string ext = Path.GetExtension(asset_path);
            if (ext.Equals( ".dll") || ext.Equals( ".cs") || 
                ext.Equals( ".meta") || ext .Equals(".js") || 
                ext.Equals( ".boo") || ext.Equals( ".jar") || 
                ext.Equals( ".mm") || ext.Equals( ".m") || 
                ext.Equals( ".asmdef") /*|| ext.Equals( ".asset")*/ || 
                ext.Equals("") || ext.Equals(".bat") )
            {
                return false;
            }

            return true;
        }

    }

#endif


}
