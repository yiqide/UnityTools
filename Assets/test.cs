using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Framework.Setting;
using Framework.SingleMone;
using Framework.Tools;
using UnityEngine;
using UnityEngine.UI;

[SingleScript(false)]
public class test : SingleMonoBase<test>, ISendEvent, IReceiveEvent
{
    void Start()
    {
        return;
        List<string> level2022_1 = new List<string>();
        List<string> level2022_2 = new List<string>();
        List<string> level2022_3 = new List<string>();
        List<string> level2022_4 = new List<string>();
        List<string> level2022_5 = new List<string>();
        List<string> level2022_6 = new List<string>();
        List<int> count_down_2022_1 = new List<int>();
        List<int> count_down_2022_2 = new List<int>();
        List<int> count_down_2022_3 = new List<int>();
        List<int> count_down_2022_4 = new List<int>();
        List<int> count_down_2022_5 = new List<int>();
        List<int> count_down_2022_6 = new List<int>();
        string str = FileTools.ReadFile("/Users/ddw/Downloads/工作簿1.csv");
        string[] strs = str.Split(new string[] {",", System.Environment.NewLine}, StringSplitOptions.None);

        for (int i = 0; i < strs.Length - 1; i += 18)
        {
            string s = strs[i + 1];
        }

        for (int i = 0; i < strs.Length - 1; i += 18)
        {
            level2022_1.Add(strs[i + 1]);
            count_down_2022_1.Add(int.Parse(strs[i + 2]));
            if (!string.IsNullOrEmpty(strs[i + 4]))
            {
                level2022_2.Add(strs[i + 4]);
            }

            if (!string.IsNullOrEmpty(strs[i + 5]))
            {
                count_down_2022_2.Add(int.Parse(strs[i + 5]));
            }

            if (!string.IsNullOrEmpty(strs[i + 7]))
            {
                level2022_3.Add(strs[i + 7]);
            }

            if (!string.IsNullOrEmpty(strs[i + 8]))
            {
                count_down_2022_3.Add(int.Parse(strs[i + 8]));
            }

            //====
            if (!string.IsNullOrEmpty(strs[i + 10]))
            {
                level2022_4.Add(strs[i + 10]);
            }

            if (!string.IsNullOrEmpty(strs[i + 11]))
            {
                count_down_2022_4.Add(int.Parse(strs[i + 11]));
            }

            if (!string.IsNullOrEmpty(strs[i + 13]))
            {
                level2022_5.Add(strs[i + 13]);
            }

            if (!string.IsNullOrEmpty(strs[i + 14]))
            {
                count_down_2022_5.Add(int.Parse(strs[i + 14]));
            }

            if (!string.IsNullOrEmpty(strs[i + 16]))
            {
                level2022_6.Add(strs[i + 16]);
            }

            if (!string.IsNullOrEmpty(strs[i + 17]))
            {
                if (strs[i + 17].Contains('!'))
                {
                    Debug.Log("???????");
                    continue;
                }

                count_down_2022_6.Add(int.Parse(strs[i + 17]));
            }
        }

        FileTools.WriteFile("/Users/ddw/Downloads/2022_1.json", SerializeTools.ObjToString(level2022_1.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_2.json", SerializeTools.ObjToString(level2022_2.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_3.json", SerializeTools.ObjToString(level2022_3.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_4.json", SerializeTools.ObjToString(level2022_4.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_5.json", SerializeTools.ObjToString(level2022_5.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_6.json", SerializeTools.ObjToString(level2022_6.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_1.json",
            SerializeTools.ObjToString(count_down_2022_1.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_2.json",
            SerializeTools.ObjToString(count_down_2022_2.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_3.json",
            SerializeTools.ObjToString(count_down_2022_3.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_4.json",
            SerializeTools.ObjToString(count_down_2022_4.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_5.json",
            SerializeTools.ObjToString(count_down_2022_5.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_6.json",
            SerializeTools.ObjToString(count_down_2022_6.ToArray()));
        return;
    }

    protected override void Awake()
    {
        base.Awake();
        this.RegisterEvent(ESendType.EAll, action);
    }

    private void action(object[] objects)
    {
        Debug.Log(objects[0]);
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            this.SendEvent(EReceiveType.EAll, "你好");
        }
    }

    public ESendType SendType => ESendType.Default;
    public EReceiveType ReceiveType => EReceiveType.Default;
}
