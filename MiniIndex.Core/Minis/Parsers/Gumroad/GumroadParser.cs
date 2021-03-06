﻿using HtmlAgilityPack;
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
            bool isGumroadUrl = url.Host.Replace("www.", "").Equals("gumroad.com", StringComparison.OrdinalIgnoreCase);

            if (!isGumroadUrl)
            {
                return false;
            }

            return IsLFormatUrl(url) || IsHashFormatUrl(url);
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            if (IsHashFormatUrl(url))
            {
                string id = url.ToString().Split('#').Last();
                url = new Uri($"https://www.gumroad.com/l/{id}");
            }

            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);

            HtmlNode creatorLink = htmlDoc.DocumentNode.SelectNodes("//a[@class=\"js-creator-profile-link\"]")
                .FirstOrDefault();

            string creatorUrl = creatorLink.GetAttributeValue("href", null);
            string creatorName = new Uri(creatorUrl).AbsolutePath.Skip(1).AsString();

            Dictionary<string, string> miniProperties = htmlDoc.DocumentNode.SelectNodes("//*[@itemprop]")
                .Select(node => new
                {
                    property = node.GetAttributeValue("itemprop", null),
                    value = GetNodeContent(node)
                })
                .Where(node => !String.IsNullOrWhiteSpace(node.property))
                .ToDictionary(k => k.property, v => v.value);

            Creator creator = new Creator
            {
                Name = creatorName
            };
            GumroadSource source = new GumroadSource(creator, creatorName);
            creator.Sites.Add(source);

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = System.Web.HttpUtility.HtmlDecode(miniProperties["name"]),
                Thumbnail = miniProperties["image"],
                Link = miniProperties["url"]
            };

            mini.Cost = Int32.Parse(miniProperties["price"].Split(".").First());
            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }

        public string GetNodeContent(HtmlNode node)
        {
            switch (node.GetAttributeValue("itemprop", null))
            {
                case "url":
                    return node.GetAttributeValue("href", null);

                case "image":
                    return node.GetAttributeValue("src", null);

                default:
                    string directText = node.GetDirectInnerText().Trim();

                    if (!String.IsNullOrWhiteSpace(directText))
                    {
                        return directText;
                    }

                    foreach (HtmlNode innerNode in node.ChildNodes)
                    {
                        string innerText = GetNodeContent(innerNode);

                        if (!String.IsNullOrWhiteSpace(innerText))
                        {
                            return innerText;
                        }
                    }
                    return node.GetAttributeValue("content", null);
            }
        }

        private static bool IsHashFormatUrl(Uri url)
        {
            return !String.IsNullOrWhiteSpace(url.Fragment)
                            && url.Fragment.StartsWith("#");
        }

        private static bool IsLFormatUrl(Uri url)
        {
            return !String.IsNullOrWhiteSpace(url.LocalPath)
                && url.LocalPath.StartsWith("/l/");
        }
    }
}