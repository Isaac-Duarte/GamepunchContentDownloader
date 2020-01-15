using Caliburn.Micro;
using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GamepunchContentDownloader.Models
{
    class FileDownload : PropertyChangedBase
    {
        private string url;
        private string fileName;
        private string fileNameDecompressed;
        private string status;
        private string filePath;
        private int progress;
        private WebClient webClient;

        /// <summary>
        /// Constuctor of the FileDownload
        /// </summary>
        public FileDownload(string url, string outPath)
        {
            Url = url;
            filePath = outPath;

            Status = "Pending";

            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }

            using (webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
                webClient.DownloadFileAsync(new Uri(url), $@"tmp\{FileName}");
            }
        }

        /// <summary>
        /// The URL of the file
        /// </summary>
        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                fileName = Path.GetFileName(url);
                fileNameDecompressed = Path.GetFileNameWithoutExtension(url);
                NotifyOfPropertyChange(() => Url);
                NotifyOfPropertyChange(() => FileName);
                NotifyOfPropertyChange(() => FileNameDecompressed);
            }
        }

        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// Name of the without .bz2
        /// </summary>
        public string FileNameDecompressed
        {
            get { return fileNameDecompressed; }
        }

        /// <summary>
        /// Status for the download.
        /// </summary>
        public string Status
        { 
            get { return status; }
            set
            {
                status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        /// <summary>
        /// Path of the download
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// Progress of the download
        /// </summary>
        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                NotifyOfPropertyChange(() => Progress);
            }
        }

        /// <summary>
        /// Webclient file progress changed aync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Status = $"Downloading {e.ProgressPercentage}%";
            Progress = e.ProgressPercentage;
        }

        /// <summary>
        /// Called when the file is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Status = "Decompressing";

            try
            {
                FileStream fileStreamRead = new FileStream($@"tmp\{FileName}", FileMode.Open);
                FileStream fileStreamWrite = new FileStream($@"{filePath}\{FileNameDecompressed}", FileMode.Create);

                Task decompressTask = Task.Run(() => BZip2.Decompress(fileStreamRead, fileStreamWrite, true));
                await decompressTask;

                File.Delete($@"tmp\{FileName}");
            }
            catch
            {
                Status = "ERROR!";
            }

            Status = "Done!";
        }
    }
}
