using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMono : MonoBehaviour 
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Dictionary<Type, Type[]> typeInterFaces = new Dictionary<Type, Type[]>();
        
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var item in assembly.GetTypes())
            {
                typeInterFaces.Add(item,item.GetInterfaces());
            }
        }

        var types = GetTypes(typeInterFaces,typeof(ISingleMonoInterface));
        foreach (var item in types)
        {
            gameObject.AddComponent<CoroutineTools>();
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
    public interface  ISingleMonoInterface
    {
        protected abstract void Init();
    }
    
}
