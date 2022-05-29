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
    /// �ļ��Ĵ洢·��
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
    /// �ڸ����ݵ�һ������ʱ����
    /// </summary>
    public virtual void Init() 
    {

    }

    /// <summary>
    /// ��������
    /// </summary>
    public virtual void Save()
    {
        FileTools.WriteFile(FilePath, SerializeTools.ObjToString(Instance));
    }

}
