﻿using HtmlAgilityPack;
using MiniIndex.Core.Utilities;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.MyMiniFactory
{
    public class MyMiniFactoryParser : IParser
    {
        public MyMiniFactoryParser(MyMiniFactoryClient myMiniFactoryClient)
        {
            _myMiniFactoryClient = myMiniFactoryClient;
        }

        private readonly MyMiniFactoryClient _myMiniFactoryClient;
        
        public string Site => "MyMiniFactory";

        public bool CanParse(Uri url)
        {
            bool isMyMiniFactoryUrl = url.Host.Replace("www.", "").Equals("myminifactory.com", StringComparison.OrdinalIgnoreCase);

            if (!isMyMiniFactoryUrl)
            {
                return false;
            }

            bool mmfFormat1 = !String.IsNullOrWhiteSpace(url.LocalPath)
                && url.LocalPath.StartsWith("/object/");

            return mmfFormat1;
        }

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            string objectId = GetObjectIdFromUrl(url);

            MyMiniFactoryModel.RootObject myModel = await _myMiniFactoryClient.GetObject(objectId);

            if (myModel is null)
            {
                return null;
            }

            Creator creator = new Creator
            {
                Name = myModel.designer.username
            };

            MyMiniFactorySource source = new MyMiniFactorySource(creator, myModel.designer.profile_url);
            creator.Sites.Add(source);

            Mini mini = new Mini
            {
                Name = myModel.name,
                Status = Status.Unindexed,
                Cost = Int32.Parse(myModel.price.value),
                Link = myModel.url,
                Creator = creator
            };

            mini.Thumbnail = myModel.images.Where(i => i.is_primary == true).FirstOrDefault().large.url;

            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }

        private string GetObjectIdFromUrl(Uri url)
        {
            return url.AbsolutePath.Split('-').Last();
        }
    }
}

/*
 
                HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url, null, null);

            HtmlNode creatorLink = htmlDoc.DocumentNode.SelectNodes("//a[@class=\"under-hover\"]")
                .FirstOrDefault();

            string creatorUrl = creatorLink.GetAttributeValue("href", null);
            string creatorName = Uri.UnescapeDataString(creatorUrl.Split('/').Last());

            Creator creator = new Creator
            {
                Name = creatorName
            };
            MyMiniFactorySource source = new MyMiniFactorySource(creator, creatorName);
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
            HtmlNodeCollection priceNode = htmlDoc.DocumentNode.SelectNodes("//span[@class=\"price-title\"]");
            if (priceNode != null)
            {
                cost = Int32.Parse(priceNode.First().InnerText.Remove(0, 1).Split(".").First());
            }
            mini.Cost = cost;

            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;

 * */
