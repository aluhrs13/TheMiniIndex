using HtmlAgilityPack;
using MiniIndex.Core.Utilities;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.Cults3d
{
    public class Wargaming3dParser : IParser
    {
        public string Site => "Wargaming3d";

        public bool CanParse(Uri url)
        {
            bool isWargaming3dUrl = url.Host.Replace("www.", "").Equals("wargaming3d.com", StringComparison.OrdinalIgnoreCase);

            if (!isWargaming3dUrl)
            {
                return false;
            }

            bool w3dFormat1 = !String.IsNullOrWhiteSpace(url.LocalPath)
                && url.LocalPath.Contains("/product/");

            return w3dFormat1;
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);


            HtmlNode creatorLink = htmlDoc.DocumentNode.SelectNodes("//a[@class='by-vendor-name-link']")
                .FirstOrDefault();

            string creatorUrl = creatorLink.GetAttributeValue("href", null);
            string creatorName = Uri.UnescapeDataString(creatorUrl.Split('/')[4]);

            Creator creator = new Creator
            {
                Name = creatorName
            };
            Wargaming3dSource source = new Wargaming3dSource(creator, creatorName);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = System.Web.HttpUtility.HtmlDecode(htmlDoc.DocumentNode.SelectNodes("//h1").FirstOrDefault().InnerText.Trim()),
                Thumbnail = htmlDoc.DocumentNode.SelectNodes("//meta").Where(n => n.Attributes.Any(a => a.Value == "og:image")).First()
                    .Attributes.Where(a => a.Name == "content").First().Value,
                Link = url.ToString()
            };

            int cost = 0;
            HtmlNodeCollection priceNode = htmlDoc.DocumentNode.SelectNodes("//div[@class='price-wrapper']");
            if (priceNode != null && !priceNode.First().InnerText.Contains("0.00"))
            {
                //TODO (GitHub #167) - Parsing cost here is a bit hard, so just setting it to be 1 for now since we only have a boolean for cost.
                cost = 1;
            }
            mini.Cost = cost;

            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }
    }
}
