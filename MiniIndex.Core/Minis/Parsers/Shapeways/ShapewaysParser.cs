using HtmlAgilityPack;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.Shapeways
{
    public class ShapewaysParser : IParser
    {
        public ShapewaysParser()
        {
        }

        public string Site => "Shapeways";

        public bool CanParse(Uri url)
        {
            return url.Host.Replace("www.", "").Equals("shapeways.com", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);

            string creatorName = htmlDoc.DocumentNode
                .SelectNodes("//a[@data-sw-tracking-link-id=\"view-profile\"]")
                .FirstOrDefault()
                .GetAttributeValue("data-sw-tracking-target-entity-id", null);

            Dictionary<string, string> miniProperties = htmlDoc.DocumentNode.SelectNodes("//div[@itemtype=\"http://schema.org/Product\"]/meta")
                .Select(node => new
                {
                    property = node.GetAttributeValue("itemprop", null),
                    content = node.GetAttributeValue("content", null)
                })
                .Where(node => !String.IsNullOrWhiteSpace(node.property))
                .ToDictionary(k => k.property, v => v.content);

            Creator creator = new Creator
            {
                Name = creatorName
            };
            ShapewaysSource source = new ShapewaysSource(creator, creatorName);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = System.Web.HttpUtility.HtmlDecode(miniProperties["name"]),
                Thumbnail = miniProperties["image"],
                Link = miniProperties["url"]
            };
            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }
    }
}