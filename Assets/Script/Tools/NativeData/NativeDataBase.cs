using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Tools;
using UnityEngine;

[Serializable]
public abstract class NativeDataBase<T> where T : new()
{
    public static T Instance
    {
        get
        {
            if (instance==null)
            {
                Rade();
                (Instance as NativeDataBase<T>)?.Init();
            }
            return instance;
        }
    }

    public static string FilePath
    {
        get
        {
            if (filePath==null)
            {
                filePath=  Application.persistentDataPath + "/NativeData/" + typeof(T).Name+".json";
            }
            return filePath;
        }
    }
    
    private static string filePath=null;
    private static T instance;
    
    public static void Rade()
    {
        try
        {
            instance =SerializeTools.StringToObj<T>(FileTools.ReadFile(filePath));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        instance ??= new T();
    }
    
    public virtual void Save()
    {
        FileTools.WriteFile(FilePath, SerializeTools.ObjToString(Instance));
    }

    public virtual void Init()
    {
        
    }

}
