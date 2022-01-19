using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileTools
{
    public static void WriteFile(string path, string data)
    {
        string savePath= Path.GetDirectoryName(path);
        if(Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
        StreamWriter streamWriter = new StreamWriter(fileStream);
        fileStream.SetLength(0);
        streamWriter.Write(data);
        streamWriter.Close();
        streamWriter.Dispose();
        fileStream.Close();
        fileStream.Dispose();
    }

    public static void WriteFile_bytes(string path, byte[] data)
    {
        string savePath = Path.GetDirectoryName(path);
        if (Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
        fileStream.SetLength(0);
        StreamWriter streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(data);
        streamWriter.Close();
        streamWriter.Dispose();
        fileStream.Close();
        fileStream.Dispose();
    }

    public static string ReadFile(string path, FileMode mode = FileMode.Open)
    {
        FileStream fileStream = new FileStream(path, mode);
        byte[] vs = new byte[1024 * 16];
        string s = "";
        while (true)
        {
            int i = fileStream.Read(vs, 0, vs.Length);
            s += System.Text.UnicodeEncoding.UTF8.GetString(vs, 0, i);
            if (i < vs.Length) break;
        }
        fileStream.Close();
        return s;
    }

    public static byte[] ReadFile_bytes(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open);
        int length = (int) fileStream.Length;
        byte[] bytes = new byte[length];
        fileStream.Read(bytes, 0, length);
        fileStream.Close();
        fileStream.Dispose();
        return bytes;
    }
    
}