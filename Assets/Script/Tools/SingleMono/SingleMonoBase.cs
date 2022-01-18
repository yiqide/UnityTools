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
    /// 将会在实例化的时候调用
    /// </summary>
    public virtual void Init()
    {
        if (instance != null)
        {
            Debug.LogWarning("你不能自己调用这个方法");
            return;
        }
        instance = (T)this;
    }

}
