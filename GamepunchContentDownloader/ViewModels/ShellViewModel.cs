using Caliburn.Micro;
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

namespace GamepunchContentDownloader.ViewModels
{
    class ShellViewModel : PropertyChangedBase
    {
        private bool canDownload;
        private string selectedValue;
        private ObservableCollection<FileDownload> downloads;
        private ScraperService scraper;
        private string outputPath;
        private Visibility progressCircleVisibility;

        /// <summary>
        /// Constuctor for the shell view model
        /// </summary>
        public ShellViewModel()
        {
            CanDownload = true;
            SelectedValue = "GamePunch Jailbreak";
            ProgressCircleVisibility = Visibility.Collapsed;
            scraper = new ScraperService();
            System.Net.ServicePointManager.DefaultConnectionLimit = 5;
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
            }
        }

        /// <summary>
        /// Selected Value for combo box
        /// </summary>
        public string SelectedValue
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
                    downloads = new ObservableCollection<FileDownload>();
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
        /// Download button click event
        /// </summary>
        public async void Download()
        {
            if (!Directory.Exists(OutputPath))
            {
                OutputPath = "Not a valid directory!";
                return;
            }

            string contentUrl = "";

            switch (SelectedValue)
            {
                case "GamePunch Jailbreak":
                    contentUrl = "http://glow.site.nfoservers.com/server/maps/";
                    break;
                case "GamePunch Minigames":
                    contentUrl = "http://gmpg.site.nfoservers.com/server/maps/";
                    break;
                default:
                    return;
            }

            CanDownload = false;
            ProgressCircleVisibility = Visibility.Visible;
            List<string> urls;

            try
            {
                urls = await scraper.ScrapeWebsite(contentUrl); 
            }
            catch (Exception e)
            {
                OutputPath = e.Message;
                return;
            }

            ProgressCircleVisibility = Visibility.Collapsed;

            foreach (string url in urls)
            {
                if (File.Exists($@"{OutputPath}\{Path.GetFileNameWithoutExtension(url)}"))
                {
                    continue;
                }

                Downloads.Add(new FileDownload($"{contentUrl}/{url}", $@"{OutputPath}\"));
            }
        }
    }
}
