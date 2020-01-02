using HtmlAgilityPack;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.Shapeways
{
    public class ShapewaysParser : IParser
    {
        public ShapewaysParser(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        public string Site => "Shapeways";

        public bool CanParse(Uri url)
        {
            return url.Host.Replace("www.", "").Equals("shapeways.com", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            Mini mini = _context.Mini.FirstOrDefault(m => m.Link == url.ToString());

            if (mini != null)
            {
                return mini;
            }

            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);

            HtmlNode creatorNode = htmlDoc.DocumentNode
                .SelectNodes("//a[@data-sw-tracking-link-id=\"view-profile\"]")
                .FirstOrDefault();

            string creatorPageRelativeLink = creatorNode.GetAttributeValue("href", null);
            string creatorName = creatorNode.GetAttributeValue("data-sw-tracking-target-entity-id", null);

            Dictionary<string, string> miniProperties = htmlDoc.DocumentNode.SelectNodes("//div[@itemtype=\"http://schema.org/Product\"]/meta")
                .Select(node => new
                {
                    property = node.GetAttributeValue("itemprop", null),
                    content = node.GetAttributeValue("content", null)
                })
                .Where(node => !String.IsNullOrWhiteSpace(node.property))
                .ToDictionary(k => k.property, v => v.content);

            Uri baseUri = new Uri("https://www.shapeways.com");
            Uri creatorPageUri = new Uri(baseUri, creatorPageRelativeLink);

            Creator creator = new Creator
            {
                Name = creatorName,
                ShapewaysURL = creatorPageUri.ToString()
            };

            mini = new Mini()
            {
                Creator = creator,
                Name = miniProperties["name"],
                Thumbnail = miniProperties["image"],
                Link = miniProperties["url"]
            };

            return mini;
        }
    }
}