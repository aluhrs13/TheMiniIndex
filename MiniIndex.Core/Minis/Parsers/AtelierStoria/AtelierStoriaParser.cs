using HtmlAgilityPack;
using MiniIndex.Core.Utilities;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.AtelierStoria
{
    public class AtelierStoriaParser : IParser
    {
        public string Site => "AtelierStoria";

        public bool CanParse(Uri url)
        {
            bool isAtelierStoriaUrl = url.Host.Replace("www.", "").Equals("atelierstoria.com", StringComparison.OrdinalIgnoreCase);

            if (!isAtelierStoriaUrl)
            {
                return false;
            }

            bool atelierStoriaFormat1 = !String.IsNullOrWhiteSpace(url.LocalPath)
                && url.LocalPath.Contains("/collections/");

            return atelierStoriaFormat1;
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);

            // atelierstoria.com only has minis created by atelierstoria
            Creator creator = new Creator
            {
                Name = "atelierstoria"
            };
            AtelierStoriaSource source = new AtelierStoriaSource(creator);
            creator.Sites.Add(source);

            // URLs come in the form 'https://atelierstoria.com/collections/collection/mini?variant=variantId'
            // Some minis have variants, in which case they have a single query parameter named variant.
            var variantId = url.Query.Split("=").Last();

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = getValueFromMeta(htmlDoc, "product:title:", variantId),
                Thumbnail = getValueFromMeta(htmlDoc, "product:image:", variantId),
                Cost = getCost(htmlDoc, variantId),
                Link = url.ToString(),
            };

            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }

        /// <summary>
        /// The page has a set of meta tags defined for this parser.
        /// Their attributes define title, image and price.
        /// A miniature can have several variants, in which case the page lists all of them in the meta tags.
        /// </summary>
        /// <param name="htmlDoc">The html document to parse.</param>
        /// <param name="target">The meta tag attribute to find, such as "product:title:".</param>
        /// <param name="variantId">The variant name of the particular miniature variant, such as "Pose-A". If the miniature doesn't have variants, variantId will be empty.</param>
        /// <returns></returns>
        private string getValueFromMeta(HtmlDocument htmlDoc, string target, string variantId)
        {
            return htmlDoc.DocumentNode.SelectNodes("//meta").Where(n => n.Attributes.Any(a => a.Value == target + variantId || a.Value == target)).First()
                    .Attributes.Where(a => a.Name == "content").First().Value.Trim();
        }

        /// <summary>
        /// Returns the price of a miniature. Free minis will be defined with an attribute of "0" and result in cost = 0
        /// </summary>
        /// <param name="htmlDoc">The html document to parse.</param>
        /// <param name="variantId">The variant name of the particular miniature variant, such as "Pose-A". If the miniature doesn't have variants, variantId will be empty.</param>
        /// <returns></returns>
        private int getCost(HtmlDocument htmlDoc, string variantId)
        {
            int cost = 0;
            string priceContent = getValueFromMeta(htmlDoc, "product:price:", variantId);
            if (priceContent != null)
            {
                cost = Int32.Parse(priceContent.Split(".").First());
            }
           return cost;
        }
    }
}
