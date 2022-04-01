using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework.Tools
{
    public static class FileTools
    {
        public static void WriteFile(string path, string data)
        {
            string savePath = Path.GetDirectoryName(path);
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

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
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

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

        public static byte[] ReadFile_bytes(string path, FileMode mode = FileMode.Open)
        {
            FileStream fileStream = new FileStream(path, mode);
            int length = (int) fileStream.Length;
            byte[] bytes = new byte[length];
            fileStream.Read(bytes, 0, length);
            fileStream.Close();
            fileStream.Dispose();
            return bytes;
        }
        
        /// <summary>
        /// 递归获取目标路径下的所有文件
        /// </summary>
        /// <param name="path">路径必须是文件夹的路径</param>
        /// <returns></returns>
        public static List<string> GetAllFile(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError("不存在的文件夹："+path);
                return null;
            }
            List<string> list = new List<string>();
            list.AddRange(Directory.GetFiles(path));
            foreach (var item in GetAllDirectory(path))
            {
                list.AddRange(Directory.GetFiles(item));
            }

            return list;
            
        }

        /// <summary>
        /// 递归获取所有目标文件下的所有文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetAllDirectory(string path)
        {
            List<string> list = new List<string>();
            GetChildDirectory(path, ref list);
            return list;
        }
        
        private static void GetChildDirectory(string path,ref List<string> directoryPaths)
        {
            if (Directory.Exists(path))
            {
                foreach (var item in Directory.GetDirectories(path))
                {
                    directoryPaths.Add(item);
                    GetChildDirectory(item,ref directoryPaths);
                }
            }
        }
    }
}