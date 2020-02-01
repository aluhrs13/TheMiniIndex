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
    public class Cults3dParser : IParser
    {
        public string Site => "Cults3d";

        public bool CanParse(Uri url)
        {
            bool isCults3dUrl = url.Host.Replace("www.", "").Equals("cults3d.com", StringComparison.OrdinalIgnoreCase);

            if (!isCults3dUrl)
            {
                return false;
            }

            bool c3dFormat1 = !String.IsNullOrWhiteSpace(url.LocalPath)
                && url.LocalPath.Contains("/3d-model/");

            return c3dFormat1;
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);


            HtmlNode creatorLink = htmlDoc.DocumentNode.SelectNodes("//a[@class='stats']")
                .FirstOrDefault();

            string creatorUrl = creatorLink.GetAttributeValue("href", null);
            string creatorName = Uri.UnescapeDataString(creatorUrl.Split('/')[3]);

            Creator creator = new Creator
            {
                Name = creatorName
            };
            Cults3dSource source = new Cults3dSource(creator, creatorName);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = htmlDoc.DocumentNode.SelectNodes("//h1").FirstOrDefault().InnerText.Trim(),
                Thumbnail = htmlDoc.DocumentNode.SelectNodes("//meta").Where(n => n.Attributes.Any(a => a.Value == "og:image")).First()
                    .Attributes.Where(a => a.Name == "content").First().Value,
                Link = url.ToString()
            };

            int cost = 0;
            HtmlNodeCollection priceNode = htmlDoc.DocumentNode.SelectNodes("//span[@class='btn-group-end btn-third']");
            if (priceNode != null)
            {
                cost = Int32.Parse(priceNode.First().InnerText.Remove(0, 3).Split(".").First());
            }
            mini.Cost = cost;

            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }
    }
}
