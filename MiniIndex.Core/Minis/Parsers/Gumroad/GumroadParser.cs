using HtmlAgilityPack;
using MiniIndex.Core.Utilities;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.Gumroad
{
    public class GumroadParser : IParser
    {
        public string Site => "Gumroad";

        public bool CanParse(Uri url)
        {
            return url.Host.EndsWith("gumroad.com", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);

            HtmlNode nameNode = htmlDoc.DocumentNode.SelectNodes("//meta[@property=\"og:title\"]").First();
            HtmlNode imageNode = htmlDoc.DocumentNode.SelectNodes("//meta[@property=\"og:image\"]").First();
            HtmlNode urlNode = htmlDoc.DocumentNode.SelectNodes("//meta[@property=\"og:url\"]").First();
            HtmlNode costNode = htmlDoc.DocumentNode.SelectNodes("//meta[@property=\"product:price:amount\"]").First();

            Uri link = new Uri(urlNode.GetAttributeValue("content", url.ToString()));

            string creatorName = link.Host.Split('.').First().Split('/').Last();

            Creator creator = new Creator
            {
                Name = creatorName
            };
            GumroadSource source = new GumroadSource(creator, creatorName);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = System.Web.HttpUtility.HtmlDecode(nameNode.GetAttributeValue("content", null)),
                Thumbnail = imageNode.GetAttributeValue("content", null),
                Link = "https://gumroad.com" + link.AbsolutePath
            };

            mini.Cost = Convert.ToInt32(Math.Round(Convert.ToDouble(costNode.GetAttributeValue("content", "0"))));
;
            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }
    }
}