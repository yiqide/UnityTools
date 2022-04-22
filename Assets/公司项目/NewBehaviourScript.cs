using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Tools;
using Newtonsoft.Json;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        var stReadLines = File.ReadLines("/Users/ddw/Downloads/工作簿1.csv");
        var readLines = stReadLines as string[] ?? stReadLines.ToArray();
        for (int i=1;i<readLines.Count();i++)
        {
            string[] str=readLines[i].Split(new string[] {","}, StringSplitOptions.None);
            string fileName = str[0] + str[1] + ".txt";
            string filePath = "/Users/ddw/Downloads/工作簿1/"+fileName;
            LTows lTows = new LTows();
            lTows.en = str[3];
            lTows.zh = str[4];
            lTows.zh_TW = str[5];
            lTows.ja = str[6];
            lTows.ko = str[7];
            lTows.fr = str[8];
            lTows.ge = str[9];
            lTows.ru = str[10];
            lTows.it = str[11];
            lTows.por = str[12];
            lTows.sp = str[13];
            lTows.du = str[14];
            lTows.vi = str[15];
            lTows.tu = str[16];
            lTows.ind = str[17];
            lTows.po = str[19];//
            lTows.th = str[20];
            lTows.gr = str[21];
            lTows.ar = str[22];
            lTows.he = str[23];
            lTows.sw = str[24];
            lTows.da = str[25];
            lTows.no = str[26];
            lTows.fi = str[27];
            lTows.ic = str[28];
            lTows.fi = str[29];
            FileTools.WriteFile(filePath,SerializeTools.ObjToString(lTows, Formatting.None));
        }
    }
    
}

/// <summary>
/// 多语言
/// </summary>
[Serializable]
public class LTows
{
    public string zh;
    public string en;
    public string zh_TW;
    public string ja;
    public string ko;
    public string fr;
    public string ru;
    public string it;

    public string ge;//德语
    public string por;//葡萄牙
    public string sp;//西班牙
    public string du;//荷兰语
    public string vi;//越南语
    public string tu;//土耳其语
    public string ind;//印尼语
    public string po;//波兰语
    public string th;//泰语
    public string gr;//希腊语
    public string ar;//阿拉伯语
        
    public string he;//希伯来语
    public string sw;//瑞典语
    public string da;//丹麦语
    public string no;//挪威语
    public string fi;//芬兰
    public string ic;//冰岛
    public LTows()
    {
        zh = "";
        en = "";
        zh_TW = "";
        ko = "";
        fr = "";
        ru = "";
        it = "";
        ge = ""; //德语
        por = ""; //葡萄牙
        sp = ""; //西班牙
        du = ""; //荷兰语
        vi = ""; //越南语
        tu = ""; //土耳其语
        ind = ""; //印尼语
        po = ""; //波兰语
        th = ""; //泰语
        gr = ""; //希腊语
        ar = ""; //阿拉伯语

        he = ""; //希伯来语
        sw = ""; //瑞典语
        da = ""; //丹麦语
        no = ""; //挪威语
        fi = ""; //芬兰
        ic = ""; //冰岛
    }
}