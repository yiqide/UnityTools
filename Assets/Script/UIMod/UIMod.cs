using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UIMod
{
    /// <summary>
    /// 在继承时请将 被继承的类放在Framework.UIMod下否则将会查早不到该类,导致界面无法被加载
    /// </summary>
    public abstract class ViewBase : MonoBehaviour
    {
        public virtual string PrefabViewPath { get; }
        public virtual EViewType ViewType{ get; }


        public void Show()
        {
            
        }

        public void Close()
        {
            
        }
    }
}