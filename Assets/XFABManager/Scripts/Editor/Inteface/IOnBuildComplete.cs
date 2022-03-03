using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XFABManager
{
    public interface IOnBuildComplete
    {
        void OnBuildComplete(string projectName,string outputPath, BuildTarget buildTarget);
    }
}


