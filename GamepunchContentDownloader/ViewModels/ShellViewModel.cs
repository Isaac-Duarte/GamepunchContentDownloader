using Caliburn.Micro;
using GamepunchContentDownloader.Models;
using GamepunchContentDownloader.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
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

        /// <summary>
        /// Constuctor for the shell view model
        /// </summary>
        public ShellViewModel()
        {
            CanDownload = true;
            SelectedValue = "Gamepunch Jailbreak";
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
        /// Download button click event
        /// </summary>
        public async void Download()
        {
            string contentUrl = "";

            switch (SelectedValue)
            {
                case "Gamepunch Jailbreak":
                    contentUrl = "http://glow.site.nfoservers.com/server/maps/";
                    break;
                case "Gamepunch Minigames":
                    break;
                default:
                    return;
            }

            CanDownload = false;

            List<string> urls =  await scraper.ScrapeWebsite(contentUrl);

            foreach (string url in urls)
            {
                Downloads.Add(new FileDownload($"{contentUrl}/{url}", "out/"));
            }
        }
    }
}
