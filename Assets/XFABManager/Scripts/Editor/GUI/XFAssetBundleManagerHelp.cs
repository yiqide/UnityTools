using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace XFABManager
{
    public class XFAssetBundleManagerHelp : EditorWindow
    {

        Rect textureRect = new Rect(0,10,291 * 0.7F ,96 * 0.7F);
        Texture logo;
        private GUIStyle style;

        private string version;

        private void Awake()
        {
            logo = AssetDatabase.LoadAssetAtPath<Texture>("Assets/XFABManager/Texture/logo_web.png");
            version = string.Format("Version {0}", XFABConst.version);
        }

        private void ConfigStyle() {
            style = new GUIStyle( GUI.skin.label);
            style.richText = true;
            style.normal.textColor =new Color(0.03f, 0.4f, 0.9f, 1);
            style.onHover.textColor = Color.white;
            style.alignment = TextAnchor.MiddleLeft;
            style.fontStyle = FontStyle.Italic;
            //style.onFocused.textColor = Color.red;
        }

        // 每秒10帧更新
        void OnInspectorUpdate()
        {
            //开启窗口的重绘，不然窗口信息不会刷新
            Repaint();
        }

        private void OnGUI()
        {

            GUI.DrawTexture(textureRect, logo);
            GUILayout.Space(textureRect.height + 20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(130);
            GUILayout.Label(version);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.Label("XFABManager 是一款管理AssetBundle的插件,我们可以通过这个插件,对项目中的AssetBundle");
            GUILayout.Label("进行模块化管理。包括 AssetBundle 的构建，优化，压缩，版本管理 以及 更新 下载 加载");
            GUILayout.Label("卸载 内置 释放 等等!");
            GUILayout.Label("更多信息可通过点击下方教程链接获取!");
            GUILayout.Space(20);
            if ( style == null ) {
                ConfigStyle();
            }
     
            DrawLink("使用教程:", "https://www.bilibili.com/video/BV1LX4y1P7AC/");
            DrawLink("更新说明:", "https://www.bilibili.com/video/BV1uX4y1w7M9/");
            DrawLink("更多教程:", "https://space.bilibili.com/258939476");
            DrawLink("插件源码:", "https://gitee.com/xianfengkeji/xfabmanager");
            GUILayout.Space(20);
            GUILayout.Label("XFABManager交流群:1058692748");

            //GUILayout.Space(20);
            GUILayout.Label("*弦风课堂制作");
        }

        private void DrawLink(string title,string url) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title,GUILayout.Width(60));

            if (GUILayout.Button(url, style )) {
                Application.OpenURL(url);
            }

            GUILayout.EndHorizontal();
        }

    }

}

