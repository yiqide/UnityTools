using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Framework.Interfase;
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
        /*
        LTow lTow = new LTow();
        lTow.en = "Get abundant rewards by completing special levels within the event time. Enjoy Easter Day with your friends!";
        lTow.zh = "在活动时间内完成对应活动关卡，获得丰厚奖励，与好友欢度复活节的美好时光吧！";
        lTow.zh_TW = "在活動時間內完成對應活動關卡，獲得豐厚獎勵，與好友歡度復活節的美好時光吧！";
        lTow.ja = "イベント時間内にイベントレベルを完了し、豊富な報酬を獲得し、友達とイースターを祝いましょう！";
        lTow.ko = "이벤트 시간 내에 이벤트를 완료하고 푸짐한 보상을 받고 친구들과 함께 멋진 부활절 시간을 보내세요!";
        lTow.fr = "Obtenez des récompenses en complétant des niveaux spéciaux pendant la durée de l'événement. Profitez du jour de Pâques!";
        lTow.de = "Erhalte Belohnungen, indem du spezielle Levels innerhalb der Eventzeit abschließt. Genießen Sie Ostern mit Ihren Freunden!";
        lTow.ru = "Получайте обильные награды, проходя специальные уровни во время события. Наслаждайтесь Пасхой с друзьями!";
        lTow.it = "Ottieni abbondanti ricompense completando livelli speciali entro il tempo dell'evento. Goditi il giorno di Pasqua con i tuoi amici!";
        lTow.pt = "Obtenha recompensas abundantes completando níveis especiais dentro do tempo do evento. Aproveite o dia de Páscoa com seus amigos!";
        lTow.es = "Obtenga abundantes recompensas completando niveles especiales dentro del tiempo del evento. ¡Disfruta del día de Pascua con tus amigos!";
        FileTools.WriteFile("/Users/ddw/Downloads/text.txt",SerializeTools.ObjToString(lTow));
        return;
    
        LTow lTow = new LTow();
        lTow.en = "Find Your Easter Egg";
        lTow.zh = "复活节彩蛋来袭";
        lTow.zh_TW = "復活節彩蛋來襲";
        lTow.ja = "イースター卵を探す";
        lTow.ko = "부활절 달걀 사냥";
        lTow.fr = "Trouvez votre œuf de Pâques";
        lTow.de = "Finden Sie Ihr Osterei";
        lTow.ru = "Найдите свое пасхальное яйцо";
        lTow.it = "Trova il tuo uovo di Pasqua";
        lTow.pt = "Encontre o seu ovo de Páscoa";
        lTow.es = "Encuentra tu huevo de Pascua";
        FileTools.WriteFile("/Users/ddw/Downloads/text.txt",SerializeTools.ObjToString(lTow));
        return;*/
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
[Serializable]
public class LTow
{
    public string zh;
    public string en;
    public string zh_TW;
    public string ja;
    public string ko;
    public string fr;
    public string de;
    public string ru;
    public string it;
    public string pt;
    public string es;

    public LTow()
    {
        zh = "";
        en = "";
        zh_TW = "";
        ko = "";
        fr = "";
        de = "";
        ru = "";
        it = "";
        pt = "";
        es = "";
    }
}
