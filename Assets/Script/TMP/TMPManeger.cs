
using System.Collections.Generic;
using Framework.SingleMone;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

[SingleScript(false)]
public class TMPManeger : SingleMonoBase<TMPManeger>
{
    [SerializeField] private TMP_FontAsset fallbackFontAsset;
    [SerializeField] public readonly string[] FallBackFontNames = new string[]
    {
        "arial.ttf",
        "msyh.ttc"//微软雅黑
    };
    protected override void Awake()
    {
        base.Awake();
        List<Font> fonts = new List<Font>();
        List<string> fontNames = new List<string>();
        fontNames.AddRange(FallBackFontNames);
        
        foreach (var item in Font.GetPathsToOSFonts())
        {
            for (int i = 0; i < fontNames.Count; i++)
            {
                if (item.Contains(fontNames[i]))
                {
                    var f= new Font(item);
                    FontEngineError engineError= FontEngine.LoadFontFace(f);
                    switch (engineError)
                    {
                        case FontEngineError.Success:
                            Debug.Log("字体加载成功:"+item);
                            break;
                        default:
                            Debug.Log(item+"字体存在问题:"+engineError);
                            break;
                    }
                    fonts.Add(f);
                    fontNames.Remove(fontNames[i]);
                    Debug.Log("将:"+item+" 作为后备字体");
                    break;
                }
            }
        }
        foreach (var item in fontNames)
        {
            Debug.Log(item+":字体没有找到");
        }
        foreach (var item in fonts)
        {
            TMP_FontAsset deftFallbackFontAsset = TMP_FontAsset.CreateFontAsset(item);
            deftFallbackFontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            fallbackFontAsset.fallbackFontAssetTable.Add(deftFallbackFontAsset);
        }
    }
}