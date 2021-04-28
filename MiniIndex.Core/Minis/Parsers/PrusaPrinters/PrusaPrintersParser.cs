using HtmlAgilityPack;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.PrusaPrinters
{
    public class PrusaPrintersParser : IParser
    {
        public PrusaPrintersParser()
        {
        }

        public string Site => "PrusaPrinters";

        public bool CanParse(Uri url)
        {
            bool isPPURL = url.Host.Replace("www.", "").Equals("prusaprinters.org", StringComparison.OrdinalIgnoreCase);

            if (!isPPURL)
            {
                return false;
            }

            bool ppFormat1 = !String.IsNullOrWhiteSpace(url.LocalPath)
                && url.LocalPath.StartsWith("/prints/");

            return ppFormat1;
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            //Prusa format is https://www.prusaprinters.org/prints/41937-tiamat-updated
            //So we split the LocalPath of url on '-' and grab the first value, then on '/' and grab the last
            string id = url.LocalPath.Split('-').First().Split('/').Last();

            HttpClient client = new HttpClient();
            
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.prusaprinters.org/graphql/");
            requestMessage.Headers.Add("ContentType", "application/json");

            //This is super hacky, but probably slightly less hacky than HTML parsing.
            //REALLY trying to avoid pulling in a full GraphQL library for one SourceSite
            string myContent = string.Format(@"{{
                ""operationName"": ""PrintProfile"",
                ""variables"":{{""id"": ""{0}""}},
                ""query"":""query PrintProfile($id: ID!) {{ print(id: $id) {{ id slug name user {{ id slug publicUsername}} images {{ filePath }} }} }}""
            }}", id);

            requestMessage.Content = new StringContent(myContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(requestMessage);
            string responseString = await response.Content.ReadAsStringAsync();

            JObject text = JObject.Parse(responseString);

            string creatorId = text["data"]["print"]["user"]["id"].ToString();
            string creatorName = text["data"]["print"]["user"]["publicUsername"].ToString();

            Creator creator = new Creator
            {
                Name = creatorName
            };
            PrusaPrintersSource source = new PrusaPrintersSource(creator, creatorId, creatorName);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = text["data"]["print"]["name"].ToString(),
                Thumbnail = "https://media.prusaprinters.org/" + text["data"]["print"]["images"].First()["filePath"].ToString(),
                Link = "https://www.prusaprinters.org/prints/" + text["data"]["print"]["id"] + "-" + text["data"]["print"]["slug"]
            };
            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }
    }
}