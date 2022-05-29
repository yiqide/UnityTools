using System.Collections;
using System.Collections.Generic;
using Framework.UIMod;

namespace Framework.UIMod
{
    [ViewData(EViewType.MainView,"MainViewpath")]
    public class MainView :ViewBase
    {
        public override EViewType ViewType { get=>EViewType.MainView; }
        public override string PrefabViewPath { get=>"MainViewpath"; }
    }
}

