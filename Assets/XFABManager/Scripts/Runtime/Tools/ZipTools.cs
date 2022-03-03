

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace XFABManager {
    public class ZipTools
    {

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="filesPath">要压缩的文件夹路径</param>
        /// <param name="zipFilePath">压缩后的zip文件路径</param>
        public static bool CreateZipFile(string filesPath, string zipFilePath)
        {

            if (!Directory.Exists(filesPath))
            {
                Debug.LogError(string.Format("Cannot find directory '{0}'", filesPath));
                return false;
            }

            // 如果压缩文件已经存在 则 删除
            if (File.Exists(zipFilePath)) {
                File.Delete(zipFilePath);
            }


            try
            {
                string[] filenames = Directory.GetFiles(filesPath);

                if (filenames.Length == 0) {

                    Debug.LogError(string.Format("file path is empty!"));
                    return false;
                }

                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {

                    s.SetLevel(9); // 压缩级别 0-9
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Exception during processing {0}", ex));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 解压zip 解压到当前文件夹
        /// </summary>
        /// <param name="zipFilePath">d:\a.zip</param>
        public static bool UnZipFile(string zipFilePath,string targetDirectory)
        {
            if (!File.Exists(zipFilePath))
            {
                Debug.LogError( string.Format( "Cannot find file '{0}'", zipFilePath));
                return false;
            }
             
            if ( !Directory.Exists(targetDirectory) ) {
                Directory.CreateDirectory(targetDirectory);
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    string fileName = Path.GetFileName(theEntry.Name);
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create( string.Format("{0}/{1}",targetDirectory, theEntry.Name) ))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }


            return true;
        }
    }

}

