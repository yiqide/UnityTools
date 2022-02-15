using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class SingleMonoAttribute : Attribute
{
    /// <summary>
    /// 一开始就创建对象
    /// </summary>
    /// <param name="startAwake">是否在一开始就创建对象</param>
    public SingleMonoAttribute(bool startAwake) 
    {
       this.startAwake = startAwake;
    }

    private bool startAwake=false;

    public bool StartAwake { get => startAwake; }
}
