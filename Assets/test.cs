using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class test : SingleMonoBase<test>
{
    public override void Init()
    {
        base.Init();
    }

    private void Awake()
    {
        
    }

    void Start()
    {
        string str = "大家好!23211231";
        string key = "nm123456";
        Debug.Log("加密前："+str);
        string r= OtherTools.EncryptDES(str,key);
        Debug.Log("加密后："+r);
        r= OtherTools.DecryptDES(r,key);
        Debug.Log("解密后："+r);
    }

    
}
