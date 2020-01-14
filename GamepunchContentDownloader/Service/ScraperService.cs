using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GamepunchContentDownloader.Service
{
    class ScraperService
    {
        private HttpClient client;

        public ScraperService()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// Scrapes the website based on the HTML
        /// </summary>
        /// <returns>A Dictionary containing the quenstion and answer</returns>
        public async Task<List<string>> ScrapeWebsite(string url)
        {
            // Create new List object
            List<string> urls = new List<string>();

            // Grab the HTML Document for prasing
            HtmlDocument document = await GetHtmlDocumentAsync(url);

            //Create new Regex object
            Regex regex = new Regex("\"(.*?)\"");

            // Create and check nodes
            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//td[contains(@class, 'indexcolname')]");

            if (nodes == null)
            {
                return urls;
            }

            // Parse the terms
            foreach (var node in nodes)
            {
                Match match = regex.Match(node.InnerHtml);

                if (node.InnerHtml.Contains("Parent Directory") || match.Value.Contains("size") || String.IsNullOrEmpty(match.Value))
                {
                    continue;
                }

                urls.Add(match.Value.Replace("\"", ""));
            }

            return urls;
        }

        /// <summary>
        /// Sends a GET request to the website and scrapes the data.
        /// </summary>
        /// <returns></returns>
        private async Task<HtmlDocument> GetHtmlDocumentAsync(string url)
        {
            // Send a GET request
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            // Read the responseMessage
            string content = await responseMessage.Content.ReadAsStringAsync();

            // Setup the HTML Docmuent
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);

            // Return the pre-loaded document.
            return htmlDocument;
        }
    }
}
