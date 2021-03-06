﻿using Caliburn.Micro;
using GamepunchContentDownloader.Helpers;
using GamepunchContentDownloader.Models;
using GamepunchContentDownloader.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GamepunchContentDownloader.ViewModels
{
    class ShellViewModel : PropertyChangedBase
    {
        private ServerData selectedValue;
        private ObservableCollection<FileDownload> downloads;
        private readonly object downloadsCollectionLock;
        private ScraperService scraper;
        private string outputPath;
        private Visibility progressCircleVisibility;
        private ObservableCollection<ServerData> serverData;
        private bool canLoad;
        private bool canDownload;
        private bool checkAll;

        /// <summary>
        /// Constuctor for the shell view model
        /// </summary>
        public ShellViewModel()
        {
            ProgressCircleVisibility = Visibility.Collapsed;
            scraper = new ScraperService();
            System.Net.ServicePointManager.DefaultConnectionLimit = 5;

            GetServerData();
        }

        private async void GetServerData()
        {
            ServerDataService dataService = new ServerDataService();
            ProgressCircleVisibility = Visibility.Visible;

            ServerData = await dataService.GetServers("https://raw.githubusercontent.com/Isaac-Duarte/GamepunchContentDownloader/master/severdata.json");
            SelectedValue = ServerData?[0];
            CanLoad = true;
            ProgressCircleVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Enables/Disables the download button
        /// </summary>
        public bool CanLoad
        {
            get { return canLoad; }
            set
            {
                canLoad = value;
                NotifyOfPropertyChange(() => CanLoad);
            }
        }

        /// <summary>
        /// Enables/Disables the download button
        /// </summary>
        public bool CanDownload
        {
            get { return canDownload; }
            set
            {
                canDownload = value;
                NotifyOfPropertyChange(() => CanDownload);
                NotifyOfPropertyChange(() => CheckAllVisibility);
            }
        }

        /// <summary>
        /// Selected Value for combo box
        /// </summary>
        public ServerData SelectedValue
        {
            get { return selectedValue; }
            set
            {
                selectedValue = value;
                NotifyOfPropertyChange(() => SelectedValue);
            }
        }

        /// <summary>
        /// List of downloads
        /// </summary>
        public ObservableCollection<FileDownload> Downloads
        {
            get
            {
                if (downloads == null)
                {
                    downloads = new AsyncObservableCollection<FileDownload>();
                }

                return downloads;
            }
            set
            {
                downloads = value;
                NotifyOfPropertyChange(() => Downloads);
            }
        }

        /// <summary>
        /// The output path for the content
        /// </summary>
        public string OutputPath
        {
            get { return outputPath; }
            set
            {
                outputPath = value;
                NotifyOfPropertyChange(() => OutputPath);
            }
        }

        /// <summary>
        /// If the maps are being loaded in from the scraper
        /// </summary>
        public Visibility ProgressCircleVisibility
        {
            get { return progressCircleVisibility; }
            set
            {
                progressCircleVisibility = value;
                NotifyOfPropertyChange(() => ProgressCircleVisibility);
            }
        }

        /// <summary>
        /// Server Data for combo box
        /// </summary>
        public ObservableCollection<ServerData> ServerData
        {
            get { return serverData; }
            set
            {
                serverData = value;
                NotifyOfPropertyChange(() => ServerData);
            }
        }

        /// <summary>
        /// Checks all of the items in the list
        /// </summary>
        public bool CheckAll
        {
            get { return checkAll; }
            set
            {
                checkAll = value;
                NotifyOfPropertyChange(() => CheckAll);

                foreach (FileDownload download in Downloads)
                {
                    download.IsChecked = value;
                }
            }
        }

        /// <summary>
        /// Visibility for the checkall checkbox
        /// </summary>
        public Visibility CheckAllVisibility
        {
            get
            {
                if (CanDownload)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Download button click event
        /// </summary>
        public async void Start()
        {
            if (!Directory.Exists(OutputPath))
            {
                OutputPath = "Not a valid directory!";
                return;
            }

            CanLoad = false;
            ProgressCircleVisibility = Visibility.Visible;
            List<string> urls;

            try
            {
                urls = await scraper.ScrapeForBzip2(SelectedValue.FastDlUrl);
            }
            catch (Exception e)
            {
                OutputPath = e.Message;
                return;
            }

            foreach (string url in urls)
            {
                if (File.Exists($@"{OutputPath}\{Path.GetFileNameWithoutExtension(url)}"))
                {
                    continue;
                }

                Downloads.Add(new FileDownload($"{SelectedValue.FastDlUrl}/{url}", $@"{OutputPath}\"));
            }

            ProgressCircleVisibility = Visibility.Collapsed;

            CanDownload = true;
        } 
            /// <summary>
            /// Handles event for the download button
            /// </summary>
            public async void Download()
        {
            ProgressCircleVisibility = Visibility.Visible;

            Task downloadTask = Task.Run(() => download());
            await downloadTask;

            ProgressCircleVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Allows for async
        /// </summary>
        private void download()
        {
            foreach (FileDownload download in Downloads.ToList())
            {

                if (!download.IsChecked)
                {
                    Downloads.Remove(download);
                    continue;
                }

                download.StartDownload();
            }

            CanDownload = false;
        }
    }
}
