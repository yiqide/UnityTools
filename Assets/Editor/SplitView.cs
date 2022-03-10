using UnityEngine;
using UnityEditor;
public class SplitView
{
    public void OnGUI(Rect position,string windowName)
    {
        GUI.Box(position,windowName);
    }

}
