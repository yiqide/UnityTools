using System;
using UnityEditor;
using UnityEngine;

//拖拽工具
public  class DragAndDropTool: EditorWindow
{
    /// <summary>
    /// 创建一个拖拽区域
    /// </summary>
    /// <param name="rect">拖拽区域</param>
    /// <param name="action">当拖拽完成时触发的回调</param>
    public static void CreationDragAndDropArea(Rect rect , Action<string[]> action,string str="")
    {
        GUI.Box(rect, str);
        var current = Event.current;
        if (current.type == EventType.DragUpdated)
        {
            if (rect.Contains(current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                current.Use();
            }
        }

        if (current.type == EventType.DragPerform)
        {
            action?.Invoke(DragAndDrop.paths);
        }
    }

    /// <summary>
    /// 创建一个拖拽区域
    /// </summary>
    /// <param name="rect">拖拽区域</param>
    /// <param name="action">当拖拽完成时触发的回调</param>
    public static void CreationDragAndDropArea(Rect rect, Action<object[]> action,string str="")
    {
        GUI.Box(rect, str);
        var current = Event.current;
        if (current.type == EventType.DragUpdated)
        {
            if (rect.Contains(current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                current.Use();
            }
        }

        if (current.type == EventType.DragPerform)
        {
            action?.Invoke(DragAndDrop.objectReferences);
        }
    }
}