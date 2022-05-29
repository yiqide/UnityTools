using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public class ll : MonoBehaviour
{
    
    public GameObject GameObject;
    void Start()
    {
        var fonts= Font.GetOSInstalledFontNames();
        foreach (var item in fonts)
        {
            string fontName = item;
            int fontSize = 36;
 
            // Create the font
            Font systemFont = Font.CreateDynamicFontFromOSFont(fontName, fontSize);
 
            // For unity text, you can then simply set the font on the text component
            Text text=Instantiate(GameObject,transform).GetComponent<Text>();
            text.gameObject.name = item;
            text.text = fontName + ":123 qwerty 你好啊!";
            text.font = systemFont;
        }

        sadas();
        Debug.Log(FontEngine.GetFaceInfo().familyName);
        //Text llll=Instantiate(GameObject,transform).GetComponent<Text>();
        //Font font=Font.CreateDynamicFontFromOSFont()
    }

    private void sadas()
    {
        var fontName = FontEngine.GetFontFaces()[0]; //.familyName;
        int fontSize = 36;
 
        // Create the font
        Font systemFont = Font.CreateDynamicFontFromOSFont(fontName, fontSize);

        Text llll=Instantiate(GameObject,transform).GetComponent<Text>();
        llll.font = systemFont;
        llll.text = "使用的字体:" + fontName+ ":123 qwerty 你好啊! ";
    }
}

