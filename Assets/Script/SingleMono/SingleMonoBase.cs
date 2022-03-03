using Framework.Interfase;
using UnityEngine;

namespace Framework.SingleMone
{
    /// <summary>
    /// 使用mono的单例需要继承此单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleMonoBase<T> : MonoBehaviour, ISingleMonoInterface where T : SingleMonoBase<T>
    {
        public static T Instance
        {
            get
            {
                if (SingleMonoManager.Instance == null) SingleMonoManager.Init();
                if (instance == null)
                {
                    //启动对应的实例化
                    SingleMonoManager.Sign(typeof(T));
                    SingleMonoManager.InstantiationMonoTarget<T>();
                }

                return instance;
            }
        }

        private static T instance;

        /// <summary>
        /// 将会在实例化的时候调用,在Unity Awake方法之后调用
        /// </summary>
        protected void Init()
        {
            if (instance != null) return;
            SingleMonoManager.Sign(GetType());
            instance = (T) this;
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                SingleMonoManager.Sign(GetType());
                instance = (T) this;
            }
            else
            {
                Destroy(this);
                Debug.LogWarning("你在场景中已经添加了重复的单例，已经销毁了");
            }
        }
    }
}