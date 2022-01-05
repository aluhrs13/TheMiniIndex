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

            HtmlNode nameNode = htmlDoc.DocumentNode.SelectNodes("//h1[@data-coyote-locator=\"product-title-header\"]").First();
            HtmlNode imageNode = htmlDoc.DocumentNode.SelectNodes("//meta[@property=\"og:image\"]").First();
            HtmlNode linkNode = htmlDoc.DocumentNode.SelectNodes("//link[@rel=\"canonical\"]").First();

            Creator creator = new Creator
            {
                Name = creatorName
            };
            ShapewaysSource source = new ShapewaysSource(creator, creatorName);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = System.Web.HttpUtility.HtmlDecode(nameNode.InnerText),
                Thumbnail = imageNode.GetAttributeValue("content", null),
                Link = linkNode.GetAttributeValue("href", url.ToString())
            };
            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }
    }
}