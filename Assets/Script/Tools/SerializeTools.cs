using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class SerializeTools
{
    public static string ObjToString<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }

    public static T StringToObj<T>(string str)
    {
        return JsonConvert.DeserializeObject<T>(str);
    }
}