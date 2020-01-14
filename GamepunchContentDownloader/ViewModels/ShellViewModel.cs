using Caliburn.Micro;
using GamepunchContentDownloader.Models;
using GamepunchContentDownloader.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamepunchContentDownloader.ViewModels
{
    class ShellViewModel : PropertyChangedBase
    {
        private bool canDownload;
        private string selectedValue;

        /// <summary>
        /// Constuctor for the shell view model
        /// </summary>
        public ShellViewModel()
        {
            CanDownload = true;
            SelectedValue = "Gamepunch Jailbreak";

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
        public void Download()
        {
            CanDownload = false;
        }
    }
}
