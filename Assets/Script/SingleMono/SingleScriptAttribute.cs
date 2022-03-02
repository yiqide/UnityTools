using System;

namespace Framework.SingleMone
{
    /// <summary>
    /// 单例的附加特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleScriptAttribute : Attribute
    {
        /// <summary>
        /// 一开始就创建对象  true-manager自动创建  false-manager不会自动创建 ,需要你自己创建
        /// 但是仍然会在你访问的时候（前提是单例是空的）自动创建对象
        /// </summary>
        /// <param name="AutomaticallyCreated">使用manager自动创建</param>
        public SingleScriptAttribute(bool AutomaticallyCreated)
        {
            this.automaticallyCreated = AutomaticallyCreated;
        }

        private bool automaticallyCreated = false;

        public bool AutomaticallyCreated
        {
            get => automaticallyCreated;
        }
    }
}