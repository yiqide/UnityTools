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
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public enum EDailyTaskConfiguration
{
    None=0,
    logGame=1,
    useHide=2,
    findDifferences=3,
    watchVideo=4,
    playEventLevel=5,
    scaleImage=6,
    playSpecialLevel=7,
    playMainLevel=8,
}
[SingleScript(false)]
public class test : SingleMonoBase<test>, ISendEvent, IReceiveEvent
{
    void Start()
    {

        string[] str = new string[] {"ni","213" };
        Debug.Log(SerializeTools.ObjToString(str));
        return;

        List<string[]> list = new List<string[]>();
        list.Add(new string[]{((int)EDailyTaskConfiguration.logGame).ToString(),"1","1","pic_dt_1.png"});
        list.Add(new string[]{((int)EDailyTaskConfiguration.watchVideo).ToString(),"1","2","pic_dt_2.png"});
        list.Add(new string[]{((int)EDailyTaskConfiguration.playMainLevel).ToString(),"50","10","pic_dt_7.png"});
        list.Add(new string[]{((int)EDailyTaskConfiguration.playEventLevel).ToString(),"10","5","pic_dt_5.png"});
        list.Add(new string[]{((int)EDailyTaskConfiguration.findDifferences).ToString(),"75","5","pic_dt_3.png"});
        list.Add(new string[]{((int)EDailyTaskConfiguration.useHide).ToString(),"10","3","pic_dt_4.png"});
        list.Add(new string[]{((int)EDailyTaskConfiguration.playSpecialLevel).ToString(),"2","3","pic_dt_8.png"});
        list.Add(new string[]{((int)EDailyTaskConfiguration.scaleImage).ToString(),"5","1","pic_dt_6.png"});
        FileTools.WriteFile("/Users/ddw/Downloads/task.json", SerializeTools.ObjToString(list, Formatting.None));
        return;
        /*
        return;
        Debug.Log( MyNativeData.FilePath);
        MyNativeData.Instance.name = "llll";
        MyNativeData.Instance.yer = 20;
        MyNativeData.Instance.Save();
        
        
        return;
        Debug.LogError(MyNativeData.Instance.name);
        Debug.LogError(MyNativeData.Instance.yer);
        Debug.LogError(MyNativeData.Instance.fasetName);

        return;
        
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
        /*
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
        */
        List<string> level2022_7 = new List<string>();
        List<string> level2022_8 = new List<string>();
        List<string> level2022_9 = new List<string>();
        List<int> count_down_2022_7 = new List<int>();
        List<int> count_down_2022_8 = new List<int>();
        List<int> count_down_2022_9 = new List<int>();
        var strings= File.ReadLines("/Users/ddw/Downloads/工作簿1.csv");
        foreach (var item in strings)
        {
            var split=item.Split(",");
            if (!string.IsNullOrEmpty(split[0]))
            {
                level2022_7.Add(split[0]);
                count_down_2022_7.Add(int.Parse(split[1]));
            }
            if (!string.IsNullOrEmpty(split[2]))
            {
                level2022_8.Add(split[2]);
                count_down_2022_8.Add(int.Parse(split[3]));
            }
            if (!string.IsNullOrEmpty(split[4])&&split[5]!="!")
            {
                level2022_9.Add(split[4]);
                count_down_2022_9.Add(int.Parse(split[5]));
            }
            
            
            
        }
        FileTools.WriteFile("/Users/ddw/Downloads/2022_7.json", SerializeTools.ObjToString(level2022_7.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_8.json", SerializeTools.ObjToString(level2022_8.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/2022_9.json", SerializeTools.ObjToString(level2022_9.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_7.json",
            SerializeTools.ObjToString(count_down_2022_7.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_8.json",
            SerializeTools.ObjToString(count_down_2022_8.ToArray()));
        FileTools.WriteFile("/Users/ddw/Downloads/count_down_2022_9.json",
            SerializeTools.ObjToString(count_down_2022_9.ToArray()));
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
