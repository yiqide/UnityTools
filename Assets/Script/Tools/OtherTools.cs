using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 其它工具
/// </summary>
public static class OtherTools 
{
    /// <summary>
    /// 通过反射来获取字段
    /// </summary>
    /// <param name="Class">要获取对象的类</param>
    /// <param name="FieldName">字段的名称</param>
    /// <param name="vale">返回的字段</param>
    /// <param name="GetBase">是否要获取到父类和接口的字段</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public static void GetField<T,T2>(ref T Class ,string FieldName,out T2 vale,bool GetBase=true)
    {
        Type type= Class.GetType();
        List<FieldInfo> fileInfos = new List<FieldInfo>();
        if (GetBase)
        {
            while (true)
            {
                var baseType =type.BaseType;
                if (baseType==typeof(object))
                {
                    break;
                }
                var Interface= type.GetInterfaces();
                var s= type.GetFields(
                    BindingFlags.Instance| 
                    BindingFlags.NonPublic|
                    BindingFlags.Default |
                    BindingFlags.Public | 
                    BindingFlags.Static);
                fileInfos.AddRange(s);
                foreach (var item in Interface)
                {
                    var i= item.GetFields(BindingFlags.Instance| 
                                          BindingFlags.NonPublic|
                                          BindingFlags.Default |
                                          BindingFlags.Public | 
                                          BindingFlags.Static);
                    fileInfos.AddRange(i);
                }
                type = baseType;
            }

            foreach (var item in fileInfos)
            {
                if (item.Name==FieldName)
                {
                    vale=(T2)item.GetValue(Class);
                    return;
                }
            }
        }
        else
        {
            var s= type.GetFields(
                BindingFlags.Instance| 
                BindingFlags.NonPublic|
                BindingFlags.Default |
                BindingFlags.Public | 
                BindingFlags.Static);
            fileInfos.AddRange(s);
            foreach (var item in fileInfos)
            {
                if (item.Name == FieldName)
                {
                    vale = (T2) item.GetValue(FieldName);
                    return;
                }
            }
        }

        vale=default;
    }
}
