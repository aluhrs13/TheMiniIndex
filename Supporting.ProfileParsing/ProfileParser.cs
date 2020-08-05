using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net.Http;
using Supporting.ProfileParsing.JSONClasses;
using System.Net;

namespace Supporting.ProfileParsing
{
    public static class ProfileParser
    {
        public static string AutoCreateKey = System.Environment.GetEnvironmentVariable("AutoCreateKey", EnvironmentVariableTarget.Process);

        [FunctionName("ProfileParser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string url = req.Query["url"];

            log.LogInformation("[Main] Looking at - " + url.ToString());
            await ParseURLAsync(log, new Uri(url));

            string responseMessage = $"Scanned {url}";
            return new OkObjectResult(responseMessage);
        }

        public static async Task ParseURLAsync(ILogger log, Uri url)
        {
            List<Uri> LinksToSubmit = new List<Uri>();
            string URLHost = url.Host.Replace("www.", "");
                
            if(URLHost.Equals("gumroad.com", StringComparison.OrdinalIgnoreCase))
            {
                log.LogInformation("[Parsing URL] Gumroad URL, looking for links...");
                GumroadParserAsync(log, url);
            }
            else
            {
                log.LogError("[Parsing URL] Invalid URL - " +url.ToString());
            }
        }

        public static async Task GumroadParserAsync(ILogger log, Uri url)
        {
            List<Uri> urlList = new List<Uri>();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);
            HtmlNode creatorNode = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'creator-profile-wrapper')]").FirstOrDefault();
            string creatorID = creatorNode.GetAttributeValue("data-user-id", null);

            if (string.IsNullOrEmpty(creatorID))
            {
                log.LogError("[Gumroad Parsing] Couldn't find creator ID");
            }
            else
            {
                log.LogInformation("[Gumroad Parsing] Found creator ID of " + creatorID);
            }

            HttpClient httpClient = new HttpClient();

            for(int from=0; from<=100; from += 9)
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://gumroad.com/discover_search?from=" + from + "&sort=newest&user_id=" + creatorID);
                HttpContent responseContent = response.Content;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    log.LogInformation("[Gumroad Parsing] Got JSON");
                    using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                    {
                        string result = await reader.ReadToEndAsync();
                        GumroadObject rootObject = JsonConvert.DeserializeObject<GumroadObject>(result);

                        htmlDoc.LoadHtml(rootObject.products_html);
                        HtmlNodeCollection nodeCollection = htmlDoc.DocumentNode.SelectNodes("//li[contains(@class,'product-card')]");

                        foreach (HtmlNode node in nodeCollection)
                        {
                            string itemID = node.GetAttributeValue("data-permalink", null);
                            Uri foundUrl = new Uri("https://gumroad.com/l/" + itemID);

                            log.LogInformation("[Gumroad Parsing] Found URL - " + foundUrl.ToString());
                            SubmitLinkAsync(log, foundUrl);
                        }
                    }
                }
                else
                {
                    log.LogError("[Gumroad Parsing] Error loading JSON - " + response.StatusCode);
                    break;
                }
            }
        }

        public static async Task SubmitLinkAsync(ILogger log, Uri url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.theminiindex.com/api/minis/create?url=" + url.ToString() + "&key=" + AutoCreateKey);
            log.LogInformation("[Mini Submission] Looking at - " + url);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    log.LogInformation("[Mini Submission] Status - " + response.StatusCode);
                    log.LogInformation("[Mini Submission] Mini URL - " + await reader.ReadToEndAsync());
                }
            }
            catch (Exception ex)
            {
                log.LogError("[Mini Submission] Exception - " + ex.Message);
            }
        }
    }
}
