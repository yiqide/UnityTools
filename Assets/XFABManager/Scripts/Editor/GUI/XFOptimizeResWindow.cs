using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


namespace XFABManager
{

    /// <summary>
    /// 优化资源界面
    /// </summary>
    public class XFOptimizeResWindow : EditorWindow
    {
        private XFABProject project;
        private GUIStyle style;
        private Rect progressRect;

        private bool isCalculating;  // 是否正在计算可优化的资源

        private float allFileCount;             // 所有文件数量
        private float alreadyCalculatedCount;   // 已经计算的数量

        private int currentBundleIndex;
        public int currentFileIndex;

        private OptimizeResTree optimizeResTree;
        private TreeViewState optimizeResState;
        MultiColumnHeaderState optimizeResMCHState;
        private Rect optimizeResRect;

        private Dictionary<string, List<string>> needOptimizeFiles = new Dictionary<string, List<string>>();

        public void Init(XFABProject project)
        {
            this.project = project;
            StartCaculateConserveResources();
        }

        private void Awake()
        {
            style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.wordWrap = true;
            progressRect = new Rect(0, 0, position.width, position.height);
        }

        private void Update()
        {

            if (isCalculating)
            {
                CaculateConserveResourcesExcute();
            }

        }

        private void OnGUI()
        {

            if (isCalculating)
            {

                GUILayout.Label(string.Format(" isCalculating : {0} ", isCalculating));
                GUILayout.Label(string.Format(" currentBundleIndex : {0} ", currentBundleIndex));
                GUILayout.Label(string.Format(" assetBundleCount : {0} ", currentBundleIndex, project.assetBundles.Count));
                GUILayout.Label(string.Format(" allFileCount : {0} ", allFileCount));
                GUILayout.Label(string.Format(" alreadyCalculatedCount : {0} ", alreadyCalculatedCount));
                GUILayout.Label(string.Format(" currentFileIndex : {0} ", currentFileIndex));
                if (currentBundleIndex < project.assetBundles.Count) {
                    GUILayout.Label(string.Format(" currentAssetBundleFileCount : {0} ", project.assetBundles[currentBundleIndex].files.Count ));
                }
                GUILayout.Label(string.Format("正在计算{0}可优化资源...{1}%", project.name, (alreadyCalculatedCount / allFileCount) * 100));
            }
            else
            {
                if (needOptimizeFiles.Count == 0)
                {
                    progressRect.Set(0, 0, position.width, position.height);
                    GUI.Label(progressRect, string.Format("未在{0}找到可优化资源的资源！", project.name), style);
                }
                else
                {

                    // 显示可优化的资源
                    if (optimizeResTree == null) {
                        optimizeResState = new TreeViewState();
                        optimizeResRect = new Rect(2, 2, position.width-4, position.height-4);

                        var headerState = OptimizeResTree.CreateDefaultMultiColumnHeaderState();// multiColumnTreeViewRect.width);
                        if (MultiColumnHeaderState.CanOverwriteSerializedFields(optimizeResMCHState, headerState))
                            MultiColumnHeaderState.OverwriteSerializedFields(optimizeResMCHState, headerState);
                        optimizeResMCHState = headerState;

                        optimizeResTree = new OptimizeResTree(optimizeResState, optimizeResMCHState, needOptimizeFiles);
                        optimizeResTree.Reload();
                    }
                    optimizeResRect.Set(2, 2, position.width - 4, position.height - 4 - 30);
                    optimizeResTree.OnGUI(optimizeResRect);
                    optimizeResRect.Set(2, 2 + optimizeResRect.height + 5, optimizeResRect.width , 20);
                    if (GUI.Button(optimizeResRect, "一键优化")) {
                        OneKeyOptimize();
                    }
                }
            }
        }

        private void StartCaculateConserveResources()
        {
            isCalculating = true;
            alreadyCalculatedCount = 0;
            CaculateAllFileCount();
            currentBundleIndex = 0;
            currentFileIndex = 0;

            needOptimizeFiles.Clear();
        }

        // 计算可优化的资源
        private void CaculateConserveResourcesExcute()
        {

            if (currentBundleIndex < project.assetBundles.Count)
            {
                XFABAssetBundle assetBundle = project.assetBundles[currentBundleIndex];

                if (currentFileIndex < assetBundle.files.Count)
                {
                    FileInfo fileInfo = assetBundle.files[currentFileIndex];

                    // 获取到这个文件 依赖的所有文件
                    string[] dependens = AssetDatabase.GetDependencies(fileInfo.AssetPath, true);
                    if (dependens.Length != 0)
                    {
                        for (int i = 0; i < dependens.Length; i++)
                        {

                            // 前提是这个文件没有被打进 AssetBundle 如果已经打进AssetBundle ，就会形成依赖 不会产生资源冗余
                            // 如果不是有效的资源文件 也不需要往后判断
                            if ( project.IsContainFile(dependens[i]) || !AssetBundleTools.IsValidAssetBundleFile(dependens[i]) ) {
                                continue;
                            }

                            if (needOptimizeFiles.ContainsKey(dependens[i]))
                            {
                                // 把 bundle_name 加进去
                                if (!needOptimizeFiles[dependens[i]].Contains(assetBundle.bundle_name))
                                {
                                    needOptimizeFiles[dependens[i]].Add(assetBundle.bundle_name);
                                }
                            }
                            else
                            {
                                needOptimizeFiles.Add(dependens[i], new List<string>() { assetBundle.bundle_name });
                            }
                        }
                    }
                    alreadyCalculatedCount++;
                    currentFileIndex++;
                }
                else
                {
                    currentFileIndex = 0;
                    currentBundleIndex++;
                }

            }
            else
            {
                isCalculating = false;

                // 计算完毕 去掉不需要优化的资源
                List<string> remove_files = new List<string>();
                foreach (string key in needOptimizeFiles.Keys)
                {
                    if (needOptimizeFiles[key].Count == 1)
                    {
                        remove_files.Add(key);
                    }
                }
                foreach (var item in remove_files)
                {
                    needOptimizeFiles.Remove(item);
                }
            }
            //Debug.Log(string.Format(" {0}/{1} ", currentBundleIndex, project.assetBundles.Count));
        }   

        // 计算所有文件的数量
        private void CaculateAllFileCount()
        {

            allFileCount = 0;
            for (int i = 0; i < project.assetBundles.Count; i++)
            {
                if (project.assetBundles[i] != null)
                {
                    if (project.assetBundles[i].files == null)
                    {
                        Debug.Log(" project.assetBundles[i].files is null ! ");
                    }
                    allFileCount += project.assetBundles[i].files.Count;
                }

            }

        }

        // 一键优化

        private void OneKeyOptimize() {

            // 创建 AssetBundle , 
            XFABAssetBundle bundle = new XFABAssetBundle(project.name);
            bundle.bundle_name = GetBundleName();
            // 把需要优化的资源 加入到 AssetBundle

            foreach (string asset_path in needOptimizeFiles.Keys)
            {
                bundle.AddFile(asset_path);
            }

            project.AddAssetBundle(bundle);

            // 优化完成 输出日志 
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("优化").Append(project.name).Append("资源完成!\n");
            stringBuilder.Append("添加AssetBundle:").Append(bundle.bundle_name).Append("包含文件:\n");

            foreach (string asset_path in needOptimizeFiles.Keys)
            {
                stringBuilder.Append(asset_path);
            }

            Debug.Log(stringBuilder.ToString());
            // 关闭界面
            this.Close();
            // 显示弹框
            EditorUtility.DisplayDialog("提示", "优化完成，更多信息请在控制台查看!", "ok");
        }


        // 获取bundlename
        public string GetBundleName()
        {

            int index = 0;
            string name = null;
            do
            {
                index++;
                name = string.Format("optimize{0}", index);
            } while (project.IsContainAssetBundleName(name));

            return name;
        }
    }
}

