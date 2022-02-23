using System;
using UnityEngine;

public abstract class SingleMonoBase<T> : MonoBehaviour, ISingleMonoInterface where T : SingleMonoBase<T>
{
    public static T Instance
    {
        get
        {
            if (SingleMonoManager.Instance == null) SingleMonoManager.Init();
            if (instance == null)
            {
                Debug.LogError(Instance.GetType() + "是空的，意料之外的错误");
            }
            return instance;
        }
    }
    private static T instance;

    /// <summary>
    /// 将会在实例化的时候调用,在Awake方法之后调用
    /// </summary>
    protected void Init()
    {
        if (instance != null)return;
        instance = (T)this;
        Awake();
        
    }

    public virtual void Awake()
    {
        SingleMonoManager.Sign(GetType());
        if (instance == null)
        {
            instance = (T)this;
        }
    }
}
