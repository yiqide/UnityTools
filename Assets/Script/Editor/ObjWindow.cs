using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class ObjWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/ObjWindow")]
    public static void ShowExample()
    {
        ObjWindow wnd = GetWindow<ObjWindow>();
        wnd.titleContent = new GUIContent("ObjWindow");
    }

    public void CreateGUI()
    {
        
        
    }
}