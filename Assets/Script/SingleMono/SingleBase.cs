using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用非mono的单例继承此类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingleBase<T> : ISingleInterface where T: SingleBase<T>
{
    public static T Instance
    {
        get
        {
            if (SingleMonoManager.Instance == null) SingleMonoManager.Init();
            if (instance == null)
            {
                SingleMonoManager.Sign(typeof(T));
                SingleMonoManager.InstantiationTarget<T>();
            }
            return instance;
        }
    }
    private static T instance;
    
    /// <summary>
    /// 将会在实例化的时候调用,在Awake方法之 前调用
    /// </summary>
    protected void Init()
    {
        if (instance != null)
        {
            Debug.LogWarning("你不能自己调用这个方法");
            return;
        }
        instance = (T)this;
        SingleMonoManager.Sign(GetType());
        Awake();
    }

    public virtual void Awake()
    {
        
    }
}
