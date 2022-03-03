#if UNITY_EDITOR 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XFABManager
{

    // 项目显示模式
    public enum ProjectsShowMode{
        Grid,  // 以格子的形式显示
        List   // 以列表的形式显示
    }

    public class XFABManagerSettings : ScriptableObject
    {
        [HideInInspector]
        public int _selectIndex;
        public int SelectIndex {
            get {
                return _selectIndex;
            }
            set {
                _selectIndex = value;
                Save();
            }
        }
        [HideInInspector]
        public List<Profile> Profiles = new List<Profile>();

        //[HideInInspector]
        public List<string> Groups = new List<string>() { "Default" };

        private static XFABManagerSettings _settings;

        public static XFABManagerSettings Settings {
            get {
                if (_settings == null) {
                    string[] assets = AssetDatabase.FindAssets("t:XFABManagerSettings");
                    if ( assets != null && assets.Length != 0 ) {
                        _settings = AssetDatabase.LoadAssetAtPath<XFABManagerSettings>(AssetDatabase.GUIDToAssetPath(assets[0]));
                    }
                }
                return _settings;
            }
        }

        // 格子的显示模式
        public ProjectsShowMode ShowMode { get; set; } 

        public Profile[] CurrentProfiles {
            get {
                return Profiles.Where(x => x.GroupName.Equals(CurrentGroup)).ToArray();
            }
        }

        /// <summary>
        /// 当前选择的组
        /// </summary>
        public string CurrentGroup {
            get {
                if (SelectIndex >= Groups.Count)
                    SelectIndex = 0;
                return Groups[SelectIndex];
            }
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        public void Save() {
            EditorUtility.SetDirty(this);
        }

        public bool IsContainsProfileName(string name,string groupName = "Default") {

            
            foreach (var item in Profiles)
            {
                if (item.name.Equals(name) && item.GroupName.Equals(groupName)) {
                    return true;
                }
            }

            return false;
        }



    }

}

#endif