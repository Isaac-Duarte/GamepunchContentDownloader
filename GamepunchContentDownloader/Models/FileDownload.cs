using Caliburn.Micro;
using GamepunchContentDownloader.Enums;
using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GamepunchContentDownloader.Models
{
    class FileDownload : PropertyChangedBase
    {
        private string url;
        private string fileName;
        private string fileNameDecompressed;
        private Status status;
        private string filePath;
        private int progress;
        private bool isCheceked;
        private WebClient webClient;

        /// <summary>
        /// Constuctor of the FileDownload
        /// </summary>
        public FileDownload(string url, string outPath)
        {
            Url = url;
            filePath = outPath;

            Status = Status.Waiting;

            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
        }

        public void StartDownload()
        {
            Status = Status.Pending;

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
        public Status Status
        { 
            get { return status; }
            set
            {
                status = value;
                NotifyOfPropertyChange(() => Status);
                NotifyOfPropertyChange(() => FormattedStatus);
                NotifyOfPropertyChange(() => CheckBoxVisibility);
            }
        }

        /// <summary>
        /// Formats the status enum for wpf
        /// </summary>
        public string FormattedStatus
        {
            get
            {
                switch (Status)
                {
                    case Status.Waiting:
                        return "Waiting";
                    case Status.Pending:
                        return "Pending";
                    case Status.Downloading:
                        return $"Downloading {Progress}%";
                    case Status.Decompressing:
                        return "Decompressing";
                    case Status.Done:
                        return "Done!";
                    case Status.Error:
                        return "Error";
                    default:
                        return "Error";
                }
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
        /// Checkbox for each item
        /// </summary>
        public bool IsChecked
        {
            get { return isCheceked; }
            set
            {
                isCheceked = value;
                NotifyOfPropertyChange(() => IsChecked);
            }
        }

        /// <summary>
        /// Visibility
        /// </summary 
        public Visibility CheckBoxVisibility
        {
            get
            {
                if (Status == Status.Waiting)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Webclient file progress changed aync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Status = Status.Downloading;
            Progress = e.ProgressPercentage;
        }

        /// <summary>
        /// Called when the file is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Status = Status.Decompressing;

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
                Status = Status.Error;
            }

            Status = Status.Done;
        }
    }
}
