using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamepunchContentDownloader.Models
{
    class ServerData : PropertyChangedBase
    {
        private string name;
        private string fastDlUrl;

        /// <summary>
        /// Name of the server
        /// </summary>
        [JsonProperty("Name")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        /// <summary>
        /// FastDL Url of the server
        /// </summary>
        [JsonProperty("FastDlUrl")]
        public string FastDlUrl
        {
            get { return fastDlUrl; }
            set
            {
                fastDlUrl = value;
                NotifyOfPropertyChange(() => FastDlUrl);
            }
        }
    }
}
