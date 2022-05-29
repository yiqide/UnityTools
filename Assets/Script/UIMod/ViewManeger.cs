using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Framework.SingleMone;
using Framework.Tools;
using UnityEngine;
using Object = System.Object;

namespace Framework.UIMod
{
    [SingleScript(false)]
    public class ViewManager : SingleMonoBase<ViewManager>
    {
        private Stack<ViewBase> viewStack = new Stack<ViewBase>();
        private Dictionary<EViewType, string> viewPaths = new Dictionary<EViewType, string>();
        private Dictionary<EViewType, ViewBase> viewBases = new Dictionary<EViewType, ViewBase>();
        protected override void Awake()
        {
            base.Awake();
            Assembly assembly = typeof(ViewBase).Assembly;
            foreach (var item in assembly.GetTypes())
            {
                if (item.IsClass && !item.IsAbstract&& item.BaseType.FullName==typeof(ViewBase).FullName)
                {
                    ViewDataAttribute viewData=item.GetCustomAttribute<ViewDataAttribute>();
                    if (viewData!=null)
                    {
                        viewPaths.Add(viewData.ViewType,viewData.ViewPath);
                    }
                }
            }

            GameObject viewRoot= new GameObject();
            viewRoot.name = "ViewRoot";
            GameObject tipLayer=new GameObject();
            GameObject mainLayer=new GameObject();
            mainLayer.name = "MainLayer";
            tipLayer.name = "TipLayer";
            mainLayer.transform.SetParent(viewRoot.transform);
            tipLayer.transform.SetParent(viewRoot.transform);
            tipLayer.AddComponent<Canvas>().sortingOrder=50;
            mainLayer.AddComponent<Canvas>().sortingOrder=0;

        }

        public void ShowView(EViewType viewType)
        {
            //生成view
            GameObject view=Resources.Load<GameObject>(viewPaths[viewType]);
            Instantiate(view);
            //viewStack.Push();
        }

        public void CloseView()
        {
            
        }
        
        public void GetTopView()
        {
            
        }

        public void ViewIsShow()
        {
            
        }
    }

    public enum EViewType
    {
        MainView,
    }

    public enum EViewLayer
    {
        
    }
}