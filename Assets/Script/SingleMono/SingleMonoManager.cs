using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SingleMonoManager : MonoBehaviour
{
    public static SingleMonoManager Instance => initialized;
    private static SingleMonoManager initialized;
    private void Awake()
    {
        if (initialized != null) return;
        gameObject.name = "SingleMonoManager";
        initialized = this;
        FindAddScript();
    }

    private static List<string> signType=new List<string>();
    public static void Sign(Type type)
    {
        if (!signType.Contains(type.FullName))
        {
            signType.Add(type.FullName);
        }
    }

    public static void Init()
    {
        if (initialized != null) return;
        GameObject go = new GameObject();
        go.AddComponent<SingleMonoManager>();
    }

    private static void FindAddScript()
    {
        DontDestroyOnLoad(initialized.gameObject);
        Dictionary<Type, Type[]> typeInterFaces = new Dictionary<Type, Type[]>();
        Assembly assembly = Assembly.GetAssembly(typeof(SingleMonoManager));
        foreach (var item in assembly.GetTypes())
        {
            if (item.IsClass && !item.IsAbstract) typeInterFaces.Add(item, item.GetInterfaces());
        }

        var singleMonoInterfaceTypes = initialized.GetTypes(typeInterFaces, typeof(ISingleMonoInterface));
        var singleInterfaceTypes=initialized.GetTypes(typeInterFaces, typeof(ISingleInterface));
        foreach (var item in singleMonoInterfaceTypes)
        {
            try
            {
                if (!signType.Contains(item.FullName))
                {
                    ISingleMonoInterface sin=(ISingleMonoInterface)initialized.gameObject.AddComponent(item);
                    var methodInfo =sin.GetType().GetMethod("Init",BindingFlags.NonPublic | BindingFlags.Instance);
                    methodInfo.Invoke(sin,null);//Invoke(sin,null);
                    Debug.Log("添加Mono单例：" + item.FullName);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception(item.FullName + ":该类型可能没有继承SingleMonoBase<T>", e));
                throw new Exception("严重错误");
            }
        }
        foreach (var item in singleInterfaceTypes)
        {
            try
            {
                if (!signType.Contains(item.FullName))
                {
                    ISingleInterface sin=(ISingleInterface) item.Assembly.CreateInstance(item.Name);
                    var methodInfo =sin.GetType().GetMethod("Init",BindingFlags.NonPublic | BindingFlags.Instance);
                    methodInfo.Invoke(sin,null);
                    Debug.Log("添加非Mono单例：" + item.FullName);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception(item.FullName + ":该类型可能没有继承SingleBase<T>", e));
                throw new Exception("严重错误");
            }
        }
    }

    private IEnumerable<Type> GetTypes(Dictionary<Type, Type[]> typeInterFaces, Type interfaceType)
    {
        foreach (var t in typeInterFaces)
        {
            foreach (var item in t.Value)
            {
                if (item == interfaceType)
                {
                    yield return t.Key;
                    break;
                }
            }
        }
    }
}
public interface ISingleMonoInterface
{
}

public interface ISingleInterface
{
}