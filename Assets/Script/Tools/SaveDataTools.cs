using UnityEngine;

public class SaveDataTools<T>
{
    private string path;
    private T data;
    private string dataName;
    public T Data
    {
        get
        {
            return data;
        }
    }
#if UNITY_EDITOR
    /// <summary>
    /// 判断是否带有SerializeField的特性
    /// </summary>
    /// <returns></returns>
    private bool IsHave()
    {
        var customAttributes = typeof(T).CustomAttributes;
        if (typeof(T).CustomAttributes == null)
        {
            Debug.LogError("要保存的类型没有\"SerializeField\" 特性，无法保存");
            return false;
        }
        foreach (var item in customAttributes)
        {
            if (item.AttributeType == typeof(SerializeField))
            {
                return true;
            }
        }
        Debug.LogError("要保存的类型没有\"SerializeField\" 特性，无法保存");
        return false;
    }
#endif

    public void ChangeData(T data)
    {
        this.data = data;
    }

    public void SaveData()
    {
        FileTools.WriteFile(path, SerializeTools.ObjToString(data));
    }

    /// <summary>
    /// 生成一个保存数据的对象
    /// </summary>
    /// <param name="dataName">数据名称，保存数据时会用到</param>
    /// <param name="data">当没有读取到数据时需要的数据对象</param>
    /// <param name="isRegenerate">当为true时将不会从磁盘里读取数据</param>
    public SaveDataTools(string dataName, T data,bool isRegenerate=false)
    {
#if UNITY_EDITOR
        IsHave();
#endif
        this.dataName = dataName;
        path = Application.persistentDataPath + "/SaveDataToolsPath/"+dataName+".json";
        if (!isRegenerate) 
        {
            try
            {
                this.data = SerializeTools.StringToObj<T>(FileTools.ReadFile(path));
            }
            catch (System.Exception e)
            {
                Debug.LogError("读取文件失败" + e);
            }
        }
        else
        {
            this.data = data;
            return;
        }
       
        if (this.data == null) 
        {
            this.data = data;
        }
    }
}
