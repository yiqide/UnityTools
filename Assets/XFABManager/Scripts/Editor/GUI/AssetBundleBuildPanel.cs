using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace XFABManager{




    public class AssetBundleBuildPanel
    {

        private Vector3 m_ScrollPosition;

        private ValidBuildTarget buildTarget;
        private ValidBuildTarget lastBuildTarget;
        GUIContent targetContent;

        private EditorWindow window;

        private string output_path;

        private List<AssetBundleBuild> bundles = new List<AssetBundleBuild>();

        private XFABProject project;

        // 是否清空文件夹
        private bool isClearFolders;
        private GUIContent clearFolderContent;

        // 是否复制到 SteaamingAssets
        private bool isCopyToStreamingAssets;
        private GUIContent copyToStreamingAssetsContent;

        // 是否把AssetBundle 压缩为zip Compressed into a zip
        private bool isCompressedIntoZip;
        private GUIContent compressedIntoZipContent;

        private string update_message;

        // 更新信息
        private GUIContent updateMessageContent;

        // 高级设置
        private bool buildOptionSetting;
        private bool[] buildAssetBundleOption;


        public AssetBundleBuildPanel(XFABProject project, EditorWindow window) {

            this.window = window;

            targetContent = new GUIContent("Build Target", "请选择要打包的目标平台!");
            buildTarget = (ValidBuildTarget)EditorUserBuildSettings.activeBuildTarget;

            //output_path = string.Format("{0}/{1}/{2}/{3}", project.out_path(b), project.name, project.version, buildTarget);

            clearFolderContent = new GUIContent("ClearFolders", string.Format("是否在打包前 删除 {0} 这个文件夹下所有的文件!", string.Format("{0}/{1}/{2}/{3}", project.out_path((BuildTarget)buildTarget), project.name, project.version, buildTarget)));

            copyToStreamingAssetsContent = new GUIContent("复制到StreamingAssets", string.Format("是否将打包完成后的 AssetBundle 文件 复制到StreamingAssets 文件夹!"));

            compressedIntoZipContent = new GUIContent("压缩资源", string.Format("是否将打包完成后的 AssetBundle 文件 压缩为.zip!"));

            updateMessageContent = new GUIContent("更新信息", "在这里可以填写本次更新了哪些内容!");
            
            buildAssetBundleOption = new bool[project.buildAssetBundleOptions.Count];
            
            this.project = project;
        }

        public void OnGUI()
        {

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            EditorGUILayout.Space();
            // 构建的目标平台
            buildTarget = (ValidBuildTarget)EditorGUILayout.EnumPopup(targetContent, buildTarget);
            EditorGUILayout.Space();

            if (lastBuildTarget != buildTarget) {
                lastBuildTarget = buildTarget;
                output_path = project.out_path((BuildTarget)buildTarget);
            }

            // 输出路径
            EditorGUILayout.LabelField("Output Path", output_path);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Show In Explorer", GUILayout.MaxWidth(125f)))
            {
                if (Directory.Exists(output_path))
                {
                    EditorUtility.RevealInFinder(output_path);
                }
                else {
                    EditorUtility.DisplayDialog("文件夹不存在!", string.Format("文件夹{0}不存在!", output_path), "ok");
                }

            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            DrawToggle(ref project.isClearFolders,ref isClearFolders, clearFolderContent);
            DrawToggle(ref project.isCopyToStreamingAssets, ref isCopyToStreamingAssets, copyToStreamingAssetsContent);
            DrawToggle(ref project.isCompressedIntoZip, ref isCompressedIntoZip, compressedIntoZipContent);


            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            GUILayout.Label(updateMessageContent, GUILayout.Width(155));
            project.update_message = EditorGUILayout.TextArea(this.project.update_message, GUILayout.Width(600), GUILayout.Height(200));

            if (project.update_message!= null && !project.update_message.Equals( update_message) ) {
                update_message = project.update_message;
                project.Save();
            }

            GUILayout.EndHorizontal();

            // 高级设置
            EditorGUILayout.Space();
            buildOptionSetting = EditorGUILayout.Foldout(buildOptionSetting, "BuildAssetBundleOptions");
            if (buildOptionSetting)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;

                for (int i = 0; i < project.buildAssetBundleOptions.Count; i++)
                {
                    BuildOptionToggleData tog = project.buildAssetBundleOptions[i];
                    tog.isOn = EditorGUILayout.ToggleLeft(tog.content, tog.isOn);

                    if ( tog.isOn != buildAssetBundleOption[i] ) {
                        buildAssetBundleOption[i] = tog.isOn;
                        project.Save();
                    }

                }

                EditorGUILayout.Space();

                EditorGUI.indentLevel = indent;
            }

            if (GUILayout.Button("Build"))
            {
                EditorApplication.delayCall += Build;
            }
            if (GUILayout.Button("优化资源"))
            {
                ConserveResources();
            }
            if (GUILayout.Button(copyToStreamingAssetsContent))
            {
                //CopyToStreamingAssets();
                if (ProjectBuild.CopyToStreamingAssets(project, (BuildTarget)buildTarget))
                {
                    this.window.ShowNotification(new GUIContent("复制成功!"));
                }
                else {
                    this.window.ShowNotification(new GUIContent("复制失败!详情请看控制台!"));
                }
            }
            if (GUILayout.Button(compressedIntoZipContent))
            {
                //CompressedIntoZip();
                if (ProjectBuild.CompressedIntoZip(project, (BuildTarget)buildTarget))
                {
                    this.window.ShowNotification(new GUIContent("压缩成功!"));
                }
                else {
                    this.window.ShowNotification(new GUIContent("压缩失败!详情请看控制台!"));
                }
            }

            EditorGUILayout.EndScrollView();


        }

        private void DrawToggle(ref bool toggleValue,ref bool lastValue,GUIContent content) {

            toggleValue = GUILayout.Toggle(toggleValue, content);
            if (toggleValue != lastValue)
            {
                lastValue = toggleValue;
                project.Save();
            }

        }

        // 构建 AssetBundle
        public void Build() {
            ProjectBuild.Build(project, (BuildTarget)buildTarget);
        }

        
        // 优化资源
        public void ConserveResources() {

            // 打开优化资源的界面 

            XFOptimizeResWindow window = EditorWindow.GetWindow<XFOptimizeResWindow>("优化资源");
            window.Init(project);
            window.Show();

            // 找到 AssetBundle 依赖的资源
        }

}


    public enum ValidBuildTarget
    {
        StandaloneOSX = 2,
        StandaloneWindows = 5,
        iOS = 9,
        Android = 13,
        StandaloneWindows64 = 19,
        WebGL = 20,
        WSAPlayer = 21,
        StandaloneLinux64 = 24,
        PS4 = 31,
        XboxOne = 33,
        tvOS = 37,
        Switch = 38,
        Lumin = 39,
        Stadia = 40
    }

    public enum CompressOptions
    {
        Uncompressed = 0,
        StandardCompression,
        ChunkBasedCompression,
    }

    

}