using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BaseEditorWindow<T> :EditorWindow where T :  EditorWindow
{
    protected float WindowHeight => position.size.y-10 <=0 ? 0: position.size.y-10;
    protected float WindowWidth => position.size.x-10 <=0 ? 0:position.size.x-10;
}
