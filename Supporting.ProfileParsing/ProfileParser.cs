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
using System.ComponentModel.Design.Serialization;

namespace Supporting.ProfileParsing
{
    public static class ProfileParser
    {
        public static string AutoCreateKey = System.Environment.GetEnvironmentVariable("AutoCreateKey", EnvironmentVariableTarget.Process);
        public static string ThingiverseKey = System.Environment.GetEnvironmentVariable("ThingiverseKey", EnvironmentVariableTarget.Process);
        public static string MyMiniFactoryKey = System.Environment.GetEnvironmentVariable("MyMiniFactoryKey", EnvironmentVariableTarget.Process);

        [FunctionName("ProfileParser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string url = req.Query["url"];

            if (string.IsNullOrEmpty(url))
            {
                return new OkObjectResult("Need URL");
            }
            else
            {
                log.LogInformation("[Main] Looking at - " + url.ToString());
                await ParseURLAsync(log, new Uri(url));
                return new OkObjectResult($"Scanned {url}");
            }


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
            else if (URLHost.Equals("thingiverse.com", StringComparison.OrdinalIgnoreCase))
            {
                log.LogInformation("[Parsing URL] Thingiverse URL, looking for links...");
                ThingiverseParserAsync(log, url);
            }
            else if (URLHost.Equals("myminifactory.com", StringComparison.OrdinalIgnoreCase))
            {
                log.LogInformation("[Parsing URL] MyMiniFactory URL, looking for links...");
                MyMiniFactoryParserAsync(log, url);
            }
            else
            {
                log.LogError("[Parsing URL] Invalid URL - " +url.ToString());
            }
        }

        public static async Task GumroadParserAsync(ILogger log, Uri url)
        {
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
                    log.LogInformation("[Gumroad Parsing] Got JSON for #"+from);
                    using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                    {
                        string result = await reader.ReadToEndAsync();
                        GumroadObject rootObject = JsonConvert.DeserializeObject<GumroadObject>(result);

                        htmlDoc.LoadHtml(rootObject.products_html);
                        HtmlNodeCollection nodeCollection = htmlDoc.DocumentNode.SelectNodes("//li[contains(@class,'product-card')]");

                        if(nodeCollection == null)
                        {
                            break;
                        }

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

        public static async Task ThingiverseParserAsync(ILogger log, Uri url)
        {
            HttpClient httpClient = new HttpClient();
            int page = 1;
            string user = url.ToString().Split('/').Last();

            while (true)
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://api.thingiverse.com/Users/" + user + "/things?access_token=" + ThingiverseKey + "&page=" + page);
                HttpContent responseContent = response.Content;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    log.LogInformation("[Thingiverse Parsing] Got JSON");
                    using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                    {
                        string result = await reader.ReadToEndAsync();

                        try
                        {
                            List<ThingiverseObject> rootObject = JsonConvert.DeserializeObject<List<ThingiverseObject>>(result);

                            if (rootObject.Count == 0)
                            {
                                log.LogInformation("[Thingiverse Parsing] All out of items!");
                                break;
                            }

                            if (rootObject == null)
                            {
                                log.LogError("[Thingiverse Parsing] No object found!");
                            }

                            foreach (ThingiverseObject item in rootObject)
                            {
                                Uri foundUrl = new Uri(item.public_url);

                                log.LogInformation("[Thingiverse Parsing] Found URL - " + foundUrl.ToString());
                                SubmitLinkAsync(log, foundUrl);
                            }

                        }
                        catch (Exception ex)
                        {
                            log.LogError("[Thingiverse Parser] Exception getting thing list - " + ex.Message);
                        }
                    }
                }
                else
                {
                    log.LogError("[Thingiverse Parsing] Error loading JSON - " + response.StatusCode);
                }

                TryWait(log, page);
                page++;
            }
        }

        public static async Task MyMiniFactoryParserAsync(ILogger log, Uri url)
        {
            HttpClient httpClient = new HttpClient();
            int page = 1;
            string user = url.ToString().Split('/').Last();

            while (true)
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://www.myminifactory.com/api/v2/users/" + user + "/objects?key=" + MyMiniFactoryKey + "&page=" + page);
                HttpContent responseContent = response.Content;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    log.LogInformation("[MyMiniFactory Parsing] Got JSON");
                    using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                    {
                        string result = await reader.ReadToEndAsync();

                        try
                        {
                            MyMiniFactoryObject rootObject = JsonConvert.DeserializeObject<MyMiniFactoryObject>(result);

                            if(rootObject.items.Length == 0)
                            {
                                log.LogInformation("[MyMiniFactory Parsing] All out of items!");
                                break;
                            }

                            if (rootObject == null)
                            {
                                log.LogError("[MyMiniFactory Parsing] No object found!");
                            }

                            foreach (Item item in rootObject.items)
                            {
                                Uri foundUrl = new Uri(item.url);

                                log.LogInformation("[MyMiniFactory Parsing] Found URL - " + foundUrl.ToString());
                                SubmitLinkAsync(log, foundUrl);
                            }

                        }
                        catch (Exception ex)
                        {
                            log.LogError("[MyMiniFactory Parser] Exception getting object list - " + ex.Message);
                        }
                    }
                }
                else
                {
                    log.LogError("[MyMiniTfactory Parsing] Error loading JSON - " + response.StatusCode);
                }

                TryWait(log, page);
                page++;
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
                    log.LogInformation("[Mini Submission] Completed (" + response.StatusCode  + ") Mini URL - " + await reader.ReadToEndAsync());
                }
            }
            catch (Exception ex)
            {
                log.LogError("[Mini Submission] Exception (" + url + ") - " + ex.Message);
            }
        }

        public static void TryWait(ILogger log, int page)
        {
            if(page % 5 == 0)
            {
                log.LogWarning("Waiting ---------------------------------------------------------");
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
