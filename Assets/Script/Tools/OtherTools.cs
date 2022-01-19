using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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
    public static void GetField<T, T2>(ref T Class, string FieldName, out T2 vale, bool GetBase = true)
    {
        Type type = Class.GetType();
        List<FieldInfo> fileInfos = new List<FieldInfo>();
        if (GetBase)
        {
            while (true)
            {
                var baseType = type.BaseType;
                if (baseType == typeof(object))
                {
                    break;
                }

                var Interface = type.GetInterfaces();
                var s = type.GetFields(
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.Default |
                    BindingFlags.Public |
                    BindingFlags.Static);
                fileInfos.AddRange(s);
                foreach (var item in Interface)
                {
                    var i = item.GetFields(BindingFlags.Instance |
                                           BindingFlags.NonPublic |
                                           BindingFlags.Default |
                                           BindingFlags.Public |
                                           BindingFlags.Static);
                    fileInfos.AddRange(i);
                }

                type = baseType;
            }

            foreach (var item in fileInfos)
            {
                if (item.Name == FieldName)
                {
                    vale = (T2) item.GetValue(Class);
                    return;
                }
            }
        }
        else
        {
            var s = type.GetFields(
                BindingFlags.Instance |
                BindingFlags.NonPublic |
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

        vale = default;
    }

    #region 加密工具

    /// <summary>
    /// 默认密钥向量
    /// </summary>
    private static byte[] Keys = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};

    /// <summary>
    /// DES加密字符串
    /// </summary>
    /// <param name="encryptString">待加密的字符串</param>
    /// <param name="encryptKey">加密密钥,要求为8位(多余的只取前8位，不能为中文)</param>
    /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
    public static string EncryptDES(string encryptString, string encryptKey)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            //byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, Keys), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            cStream.Dispose();
            mStream.Dispose();
            return Convert.ToBase64String(mStream.ToArray());
        }
        catch
        {
            return encryptString;
        }
    }

    /// <summary>
    /// DES解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
    /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
    public static string DecryptDES(string decryptString, string decryptKey)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
            //byte[] rgbIV = Keys;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, Keys), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            cStream.Dispose();
            mStream.Dispose();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        catch
        {
            return decryptString;
        }
    }

    #endregion
}