using System;
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
                (instance as NativeDataBase<T>).Init();
            }
            return instance;
        }
    }
    /// <summary>
    /// 文件的存储路径
    /// </summary>
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
    
    private static void Rade()
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
    /// <summary>
    /// 在该数据第一次生成时调用
    /// </summary>
    public virtual void Init() 
    {

    }

    /// <summary>
    /// 保存数据
    /// </summary>
    public virtual void Save()
    {
        FileTools.WriteFile(FilePath, SerializeTools.ObjToString(Instance));
    }

}
