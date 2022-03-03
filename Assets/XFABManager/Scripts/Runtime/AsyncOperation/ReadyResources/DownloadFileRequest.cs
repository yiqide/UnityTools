using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace XFABManager
{

    /// <summary>
    /// 下载文件的请求
    /// </summary>
    public class DownloadFileRequest : CustomAsyncOperation<DownloadFileRequest>
    {

        public const string tempSuffix = ".tempFile";
        private static int CurrentDownloadFileCount = 0;
        private const int MAX_DOWNLOAD_FILE_COUNT = 6; // 同时最多下载的文件数量

        private string file_url;
        private string localfile;
        private long length;
        private string tempfile;                // 下载的临时文件路径
        private float lastProgress = 0;



        /// <summary>
        /// 下载速度 字节/每秒
        /// </summary>
        public int Speed { get; private set; }

        public DownloadFileRequest(string file_url, string localfile, long length = 0)
        {
            this.file_url = file_url;
            this.localfile = localfile;
            this.length = length;
            tempfile = string.Format("{0}{1}", localfile, tempSuffix);
        }

        private IEnumerator Download()
        {

            // 同时下载的文件数量过多 需要等待
            while (CurrentDownloadFileCount > MAX_DOWNLOAD_FILE_COUNT)
            {
                yield return null;
            }

            CurrentDownloadFileCount++;

            UnityWebRequest downloadFile = UnityWebRequest.Get(this.file_url);
            DownloadHandlerFile handlerFile = new DownloadHandlerFile(tempfile);
            downloadFile.downloadHandler = handlerFile;
            UnityWebRequestAsyncOperation asyncOperation = downloadFile.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                yield return null;
                progress = downloadFile.downloadProgress;

                // 计算网速
                Speed = (int)((progress - lastProgress) * length / Time.deltaTime);

                lastProgress = progress;
            }

            if (!string.IsNullOrEmpty(downloadFile.error))
            {
                Completed(downloadFile.error);
                yield break;
            }

            // 说明已经下载完成 但是没有把临时文件转成正式文件
            if (File.Exists(localfile))
                File.Delete(localfile);
            File.Move(tempfile, localfile);

            Completed();
        }
        protected override void OnCompleted()
        {
            base.OnCompleted();
            CurrentDownloadFileCount--;
        }

        public static DownloadFileRequest Download(string file_url, string localfile, long length = 0)
        {

            string key = string.Format("DownloadFileRequest:{0}", file_url);
            return AssetBundleManager.ExecuteOnlyOnceAtATime<DownloadFileRequest>(key, () => {
                DownloadFileRequest request = new DownloadFileRequest(file_url, localfile, length);
                CoroutineStarter.Start(request.Download());
                return request;
            });

        }

        //public static DownloadFileRequest DownloadFile(string file_url, string localfile)
        //{
        //    string key = string.Format("DownloadFile:{0}", file_url);
        //    return AssetBundleManager.ExecuteOnlyOnceAtATime<DownloadFileRequest>(key, () => {
        //        DownloadFileRequest downloadFileRequest = new DownloadFileRequest();
        //        CoroutineStarter.Start(downloadFileRequest.Download(file_url, localfile));
        //        return downloadFileRequest;
        //    });
        //}


    }

}

