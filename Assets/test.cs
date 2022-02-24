using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[Single(false)]
public class test : SingleMonoBase<test>
{
    public Image _gameObject1;
    public Image GameObject2;
    public Shader Shader;

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
        string str= FileTools.ReadFile("/Users/ddw/Downloads/工作簿1.csv");
        string[] strs= str.Split(new string[]{",",System.Environment.NewLine}, StringSplitOptions.None);

        for (int i = 0; i < strs.Length-1; i+=18)
        {
            string s = strs[i + 1];
        }
        
        for (int i = 0; i < strs.Length-1; i+=18)
        {
            level2022_1.Add(strs[i+1]);
            count_down_2022_1.Add(int.Parse(strs[i+2]));
            if (!string.IsNullOrEmpty( strs[i+4]))
            {
                level2022_2.Add(strs[i+4]);
            }
            if (!string.IsNullOrEmpty(strs[i+5]))
            {
                count_down_2022_2.Add(int.Parse(strs[i+5]));
            }
            
            if (!string.IsNullOrEmpty( strs[i+7]))
            {
                level2022_3.Add(strs[i+7]);
            }
            if (!string.IsNullOrEmpty(strs[i+8]))
            {
                count_down_2022_3.Add(int.Parse(strs[i+8]));
            }
            //====
            if (!string.IsNullOrEmpty( strs[i+10]))
            {
                level2022_4.Add(strs[i+10]);
            }
            if (!string.IsNullOrEmpty(strs[i+11]))
            {
                count_down_2022_4.Add(int.Parse(strs[i+11]));
            }
            if (!string.IsNullOrEmpty( strs[i+13]))
            {
                level2022_5.Add(strs[i+13]);
            }
            if (!string.IsNullOrEmpty(strs[i+14]))
            {
                count_down_2022_5.Add(int.Parse(strs[i+14]));
            }if (!string.IsNullOrEmpty( strs[i+16]))
            {
                level2022_6.Add(strs[i+16]);
            }
            if (!string.IsNullOrEmpty(strs[i+17]))
            {
                if (strs[i + 17].Contains('!'))
                {
                    Debug.Log("???????");
                    continue;
                }
                count_down_2022_6.Add(int.Parse(strs[i+17]));
            }
        }
        
        FileTools.WriteFile("/Users/ddw/Downloads/2022_1.json",SerializeTools.ObjToString(level2022_1.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_2.json",SerializeTools.ObjToString(level2022_2.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_3.json",SerializeTools.ObjToString(level2022_3.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_4.json",SerializeTools.ObjToString(level2022_4.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_5.json",SerializeTools.ObjToString(level2022_5.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_6.json",SerializeTools.ObjToString(level2022_6.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_1.json",SerializeTools.ObjToString(count_down_2022_1.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_2.json",SerializeTools.ObjToString(count_down_2022_2.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_3.json",SerializeTools.ObjToString(count_down_2022_3.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_4.json",SerializeTools.ObjToString(count_down_2022_4.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_5.json",SerializeTools.ObjToString(count_down_2022_5.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_6.json",SerializeTools.ObjToString(count_down_2022_6.ToArray()));

        return;



        int[] strings = new int[]
        {
           60,60,60,60,60,60,60,60,60,60,
           60,60,60,60,60,60,60,60,60,60,
           60,60,60,60,60,60,60,60,60,60,
        };
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_1.json",SerializeTools.ObjToString(strings));
        return;
        Material material = new Material(Shader);
        material.SetFloat("_Gray",1);
        _gameObject1.material=material;
        material = new Material(Shader);
        material.SetFloat("_Gray",1);
        GameObject2.material=material;
        var timeSpan = TimeZoneInfo.Local.GetUtcOffset(System.DateTime.Now);
        string offset="";
        if (timeSpan.Hours >= 0 && timeSpan.Hours<10)
        {
            offset = "+0" + timeSpan.Hours;
        }

        if (timeSpan.Hours<0&&timeSpan.Hours>-10)
        {
            offset = "-0" + Math.Abs(timeSpan.Hours);
        }

        if (timeSpan.Hours>=10)
        {
            offset = "+" + timeSpan.Hours;
        }

        if (timeSpan.Hours<=-10)
        {
            offset = timeSpan.Hours.ToString();
        }
        offset += ":00";
        Debug.Log("offset:"+offset);
        NetworkTools.AddTask("/Users/ddw/Downloads/ls.txt",
            "https://saas.castbox.fm/tool/api/v1/system/time?pattern=yyyy-MM-dd&offsetId="+offset,
            (b) =>
            {
                if (b)
                {
                   Debug.Log(FileTools.ReadFile("/Users/ddw/Downloads/ls.txt"));
                   _dateTime = new DateTime();
                }
            });
    }

    public override void Awake()
    {
        base.Awake();
        Debug.Log("你好");
    }
    

    private DateTime _dateTime;
    private int h=0;
    private int m=0;
    private float s=0;

    public int H
    {
        get => h;
        set
        {
            h= value;
        }
    }

    public int M
    {
        get => m;
        set
        {
            m = value;
            if (m>=60)
            {
                H++;
                m = m - 60;
            }

            _dateTime.AddMinutes(1);
        }
    }

    public float S
    {
        get => s;
        set
        {
            s = value;
            if (s>=60)
            {
                M++;
                s = s - 60;
            }
        }
    }

    private void Update()
    {
        
        if (Input.anyKey)
        {
            try
            { 
              Debug.Log(  MessageTools.Instance==null);

            }
            catch (Exception e)
            {
            }
           
        }
    }
}
