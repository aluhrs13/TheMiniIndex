using HtmlAgilityPack;
using MiniIndex.Core.Utilities;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.DuncanShadow
{
    public class DuncanShadowParser : IParser
    {
        public string Site => "DuncanShadow";

        public bool CanParse(Uri url)
        {
            bool DuncanShadowUrl = url.Host.Replace("www.", "").Equals("duncanshadow.com", StringComparison.OrdinalIgnoreCase);

            if (!DuncanShadowUrl)
            {
                return false;
            }

            bool DuncanShadowFormat1 = !String.IsNullOrWhiteSpace(url.LocalPath)
                && url.LocalPath.Contains("/product-page/");

            return DuncanShadowFormat1;
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);

            Creator creator = new Creator
            {
                Name = "DuncanShadow"
            };
            DuncanShadowSource source = new DuncanShadowSource(creator);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = htmlDoc.DocumentNode.SelectNodes("//meta").Where(n => n.Attributes.Any(a => a.Value == "og:title")).First()
                    .Attributes.Where(a => a.Name == "content").First().Value.Split('|')[0].Trim(),
                Thumbnail = htmlDoc.DocumentNode.SelectNodes("//meta").Where(n => n.Attributes.Any(a => a.Value == "og:image")).First()
                    .Attributes.Where(a => a.Name == "content").First().Value,
                Link = url.ToString()
            };

            mini.Cost = Int32.Parse(htmlDoc.DocumentNode.SelectNodes("//meta").Where(n => n.Attributes.Any(a => a.Value == "product:price:amount")).First()
                    .Attributes.Where(a => a.Name == "content").First().Value);

            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }
    }
}
