using GamepunchContentDownloader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GamepunchContentDownloader.Service
{
    class ServerDataService
    {
        private HttpClient httpClient;

        public ServerDataService()
        {
            httpClient = new HttpClient();
        }

        /// <summary>
        /// Sends a GET request to the website and scrapes the data.
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<ServerData>> GetServers(string url)
        {
            // Send a GET request
            HttpResponseMessage responseMessage = await httpClient.GetAsync(url);

            // Read the responseMessage
            string content = await responseMessage.Content.ReadAsStringAsync();

            // Convert the content to SeverData object
            return JsonConvert.DeserializeObject<ObservableCollection<ServerData>>(content);
        }

    }
}