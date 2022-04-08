// Magica Cloth.
// Copyright (c) MagicaSoft, 2020-2022.
// https://magicasoft.jp
using UnityEditor;
using UnityEngine;

namespace MagicaCloth
{
    /// <summary>
    /// 物理マネージャのエディタ拡張
    /// </summary>
    [CustomEditor(typeof(MagicaPhysicsManager))]
    public class MagicaPhysicsManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            MagicaPhysicsManager scr = target as MagicaPhysicsManager;

            serializedObject.Update();
            Undo.RecordObject(scr, "PhysicsManager");

            MainInspector();
            EventInspector();

            serializedObject.ApplyModifiedProperties();
        }

        void MainInspector()
        {
            MagicaPhysicsManager scr = target as MagicaPhysicsManager;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Update", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                var prop = serializedObject.FindProperty("updateTime.updatePerSeccond");
                EditorGUILayout.PropertyField(prop,new GUIContent("物理引擎更新的次数"));

                var prop2 = serializedObject.FindProperty("updateTime.updateMode");
                EditorGUILayout.PropertyField(prop2);

                // 以下は遅延実行時のみ
                if (scr.UpdateTime.IsDelay)
                {
                    var prop3 = serializedObject.FindProperty("updateTime.futurePredictionRate");
                    EditorGUILayout.PropertyField(prop3);
                }

                Help1();
            }

            // 高速書き込み
#if UNITY_2021_2_OR_NEWER
            const bool useFasterWrite = true;
#else
            const bool useFasterWrite = false;
#endif
            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying || useFasterWrite == false))
            {
                var prop4 = serializedObject.FindProperty("useFasterWrite");
                EditorGUILayout.PropertyField(prop4, new GUIContent("快速写入[实验]"));
            }
            EditorGUILayout.HelpBox("Faster Write是一个带有新的顶点缓冲区的快速网格写入。\n在Unity 2021.2或更高版本中可用", MessageType.Info);
        }

        void Help1()
        {
            MagicaPhysicsManager scr = target as MagicaPhysicsManager;

            if (scr.UpdateMode == UpdateTimeManager.UpdateMode.OncePerFrame)
            {
                EditorGUILayout.HelpBox("[OncePerFrame] must have stable FPS.必须有稳定的FPS", MessageType.Info);
            }
            else if (scr.UpdateTime.IsDelay)
            {
                EditorGUILayout.HelpBox(
                    "Delayed execution. [experimental]\n" +
                    "Improve performance by running simulations during rendering.\n" +
                    "Note, however, that the result is one frame late.\n" +
                    "This delay is covered by future predictions.\n"
                    +"延迟执行[实验]\n"+
                    "在渲染过程中通过运行模拟来提高性能\n"+
                    "但是请注意，结果是延迟了一帧\n"+
                    "这一延迟被未来的预测所掩盖",
                    MessageType.Info);
            }
        }

        private void EventInspector()
        {
            MagicaPhysicsManager scr = target as MagicaPhysicsManager;
            var pre = serializedObject.FindProperty("OnPreUpdate");
            var post = serializedObject.FindProperty("OnPostUpdate");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(pre);
            EditorGUILayout.PropertyField(post);
        }
    }
}
