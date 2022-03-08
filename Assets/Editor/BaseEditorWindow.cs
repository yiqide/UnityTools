using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseEditorWindow<T> :EditorWindow where T :  EditorWindow
{
    protected int WindowHeight => Screen.height/2-10 <=0 ? 0:Screen.height/2-10;
    protected int WindowWidth => Screen.width/2-10 <=0 ? 0:Screen.width/2-10;
}
