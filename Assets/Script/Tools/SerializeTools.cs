using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework.Tools
{
    public static class SerializeTools
    {
        public static string ObjToString<T>(T obj,Formatting indented = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(obj, indented);
        }

        public static T StringToObj<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}