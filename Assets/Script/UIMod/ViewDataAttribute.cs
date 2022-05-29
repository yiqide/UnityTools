using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UIMod
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ViewDataAttribute : Attribute
    {
        /// <summary>
        /// 可以判断view 的类型和预制体的路径
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="viewPath"></param>
        public ViewDataAttribute(EViewType viewType,string viewPath)
        {
            this.viewType = viewType;
            this.viewPath = viewPath;
        }

        private EViewType viewType;
        private string viewPath;
        public EViewType ViewType=>viewType;
        public string ViewPath=>viewPath;
    }
}
