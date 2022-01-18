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

        var types = initialized.GetTypes(typeInterFaces, typeof(ISingleMonoInterface));
        foreach (var item in types)
        {
            try
            {
                ((ISingleMonoInterface)initialized.gameObject.AddComponent(item)).Init();
                Debug.Log("添加单例：" + item.FullName);
            }
            catch (Exception e)
            {

                Debug.LogException(new Exception(item.FullName + ":该类型可能没有继承SingleMonoBase<T>", e));
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

    public abstract void Init();
}