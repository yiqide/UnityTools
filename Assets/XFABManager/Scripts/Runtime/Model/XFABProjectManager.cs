#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
 

namespace XFABManager
{

    // 监听 资源删除 和 导入资源的事件 TODO

    public class XFABProjectManager
    {
 

        #region 变量
        // 保存所有的 资源项目
        private List<XFABProject> projects;

        private static XFABProjectManager _instance;

        private double lastRefreshTime;


        private string[] allAssets;

 

        #endregion

        #region 属性
        public static XFABProjectManager Instance
        {

            get
            {
                if (_instance == null)
                {
                    _instance = new XFABProjectManager();
                }

                return _instance;
            }

        }

        public List<XFABProject> Projects
        {
            get { return projects; }
        }

        //public bool IsRefreshing {

        //    get { return isRefreshing; }

        //}

        //public float RefreshProgress
        //{

        //    get
        //    {

        //        return (float)refreshIndex / (float)allAssets.Length;
        //    }
        //}

        #endregion

        #region 方法


        // 私有构造函数
        private XFABProjectManager()
        {

            //Debug.Log("XFABManager Init!");

            InitProjects();

            //EditorApplication.update += Update;

        }

        // 初始化项目
        private void InitProjects()
        {
            projects = new List<XFABProject>();
            // 刷新项目
            RefreshProjects();

        }

        // 刷新Project
        public void RefreshProjects()
        {
            //Debug.Log(" RefreshProjects ");
            if (projects == null)
            {
                return;
            }

            if (EditorApplication.timeSinceStartup - lastRefreshTime < 0.1f) {
                return;
            }


            projects.Clear();
            // 读项目配置文件
            allAssets = AssetDatabase.FindAssets("t:XFABProject" );

            for (int i = 0; i < allAssets.Length; i++)
            {
                XFABProject project = AssetDatabase.LoadAssetAtPath<XFABProject>(AssetDatabase.GUIDToAssetPath(allAssets[i]));
                if (project != null)
                {
                    AddProject(project);
                }
            }

            lastRefreshTime = EditorApplication.timeSinceStartup;
        }

        

        // 判断是否包含某个项目
        public bool IsContainProject(string name) {

            if (projects != null) {

                for (int i = 0; i < projects.Count; i++)
                {
                    if ( projects[i].name == name ) {
                        return true;
                    }
                }
            }

            return false;
        }
 
        // 添加项目
        public void AddProject(XFABProject project) {

            if ( project == null ) 
            {
                Debug.LogError("不能添加空项目!");
                return;
            }

            if (!IsContainProject(project.name))
            {
                projects.Add(project);
            }
            else {
                Debug.LogError(string.Format("添加项目{0}失败,已存在!", project.name));
            }
        }

        // 获取项目
        public XFABProject GetProject(string name) 
        {

            if (string.IsNullOrEmpty(name)) {
                return null;
            }

            XFABProject project = null;

            for (int i = 0; i < projects.Count; i++)
            {
                if ( name.Equals( projects[i].name ) ) {
                    project = projects[i];
                }
            }

            return project;
        }

        // 是否有 项目 依赖 这个 项目
        public bool IsHaveProjectDependence(string name) {

            for (int i = 0; i < projects.Count; i++)
            {
                if ( !projects[i].name .Equals( name ) ) {
                    if (projects[i].IsDependenceProject(name)) {
                        return true;
                    }
                }
            }

            return false;
        }

        //public void Update()
        //{
        //    // 具体更新的操作
        //    RefreshProjectsExcute();
        //}

#endregion

    }


}

#endif