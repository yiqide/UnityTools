using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XFABManager
{

    public class DownloadObjectInfo {

        public string url;
        public string localfile;
        public long length;

        public DownloadObjectInfo(string url,string localfile,long length=0) {
            this.url = url;
            this.localfile = localfile;
            this.length = length;
        }

    }

    /// <summary>
    /// 下载多个文件的请求 
    /// 用来替换 DownloadManager
    /// </summary>
    public class DownloadFilesRequest : CustomAsyncOperation<DownloadFilesRequest>
    {

        //private Dictionary<string, string> files = new Dictionary<string, string>();

        private List<DownloadObjectInfo> downloadObjects = null;

        public long Speed { get; private set; }

        public DownloadFilesRequest(List<DownloadObjectInfo> downloadObjects) {
            this.downloadObjects = downloadObjects;
        }

       

        public IEnumerator Download()
        {

            if (downloadObjects == null || downloadObjects.Count == 0)
            {

                Debug.LogWarning("需要下载的文件为空,请添加后重试!");
                Completed();
                yield break;
            }

            int index = 0;
            foreach (var info in downloadObjects)
            {

                //string file_url = key;
                string localfile = info.localfile ;

                if (string.IsNullOrEmpty(info.url) || string.IsNullOrEmpty(localfile)) { continue; }
                DownloadFileRequest download = DownloadFileRequest.Download(info.url, localfile,info.length);

                while (!download.isDone)
                {
                    yield return null;
                    progress = (float)(index + download.progress) / downloadObjects.Count;
                    Speed = download.Speed;
                    //CurrentSpeedFormatStr = downloadTool.CurrentSpeedFormatStr;
                }

                if (!string.IsNullOrEmpty(download.error))
                {
                    error = string.Format("下载文件失败:{0} url:{1}", download.error, info.url);
                    Completed();
                    yield break;
                }


                index++;

            }
            Completed();
        }

        [System.Obsolete("该方法已经过时,请使用DownloadFilesRequest.DownloadFiles(List<DownloadObjectInfo> files) 代替!",true)]
        public static DownloadFilesRequest DownloadFiles(Dictionary<string,string> files)
        {
            //DownloadFilesRequest downloadFiles = new DownloadFilesRequest(null);
            ////downloadFiles.AddRange(files);
            //CoroutineStarter.Start(downloadFiles.Download());
            return null;
        }

        public static DownloadFilesRequest DownloadFiles(List<DownloadObjectInfo> files)
        {
            DownloadFilesRequest downloadFiles = new DownloadFilesRequest(files);
            CoroutineStarter.Start(downloadFiles.Download());
            return downloadFiles;
        }

    }

}

