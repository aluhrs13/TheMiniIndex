﻿using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Core.Minis;
using MiniIndex.Models;
using MiniIndex.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MiniIndex.Core.Submissions
{
    public class MiniSubmissionHandler : IRequestHandler<MiniSubmissionRequest, Mini>
    {
        public MiniSubmissionHandler(MiniIndexContext context, IEnumerable<IParser> parsers)
        {
            _context = context;
            _parsers = parsers;
        }

        private readonly MiniIndexContext _context;
        private readonly IEnumerable<IParser> _parsers;

        public async Task<Mini> Handle(MiniSubmissionRequest request, CancellationToken cancellationToken)
        {
            Uri uri = new Uri(request.Url);

            Mini mini = await _context.Set<Mini>().SingleOrDefaultAsync(m => m.Link == request.Url, cancellationToken);

            if (mini != null)
            {
                return mini;
            }

            var parser = _parsers.FirstOrDefault(p => p.CanParse(uri));

            if (parser is null)
            {
                //valid URL, but not currently supported
                //TODO: log when this happens?
                return null;
            }

            mini = await parser.ParseFromUrl(uri);
            mini.User = request.User;

            _context.Add(mini);
            await _context.SaveChangesAsync();

            return mini;
        }

        private async Task<Mini> ParseGumroad(string URL)
        {
            string parsedURL = "https://gumroad.com/products/" + URL.Split('#')[1] + "/display";

            //Initialize HTML Agility Pack variables
            string html = parsedURL;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(html);

            //Parse out creator URL
            Creator creator = new Creator
            {
                WebsiteURL = URL.Split('?')[0],
                Name = URL.Split('?')[0].Split('/').Last()
            };

            HtmlNode titleNode = htmlDoc.DocumentNode.Descendants("h1").First();
            HtmlNode imageNode = htmlDoc.DocumentNode.Descendants("img").First();
            HtmlNode costNode = htmlDoc.DocumentNode.Descendants("strong").First();

            Mini mini = new Mini
            {
                Creator = creator,
                Thumbnail = imageNode.GetAttributeValue("src", ""),
                Name = titleNode.InnerText,
                Link = URL,
                Cost = (costNode.InnerText == "$0+") ? 0 : (int)Math.Ceiling(Double.Parse(costNode.InnerText.Substring(1)))
            };

            //Check if it exists
            if (_context.Mini.Any(m => m.Link == mini.Link))
            {
                return _context.Mini.First(m => m.Link == mini.Link);
            }

            return null;
        }

        private async Task<Mini> ParseShapeways(string URL)
        {
            string html = URL;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(html);

            HtmlNode imageDiv = htmlDoc.GetElementbyId("slideshow-big");
            HtmlNode imageNode = imageDiv.ChildNodes.Where(cn => cn.Name == "img").First();

            string titleString = "product-title-header";
            HtmlNode nameNode = htmlDoc.DocumentNode.SelectNodes("//body//h1[@data-coyote-locator='" + titleString + "']").Last();

            string creatorURLString = "view-profile";
            HtmlNode creatorUrlNode = htmlDoc.DocumentNode.SelectNodes("//body//a[@data-sw-tracking-link-id='" + creatorURLString + "']").Last();
            string RelativeShapewaysURL = creatorUrlNode.Attributes.Where(att => att.Name == "href").First().Value;

            Creator creator = new Creator
            {
                ShapewaysURL = $"https://www.shapeways.com{RelativeShapewaysURL}"
            };

            Mini mini = new Mini()
            {
                Creator = creator,
                Name = nameNode.InnerText,
                Thumbnail = imageNode.GetAttributeValue("src", ""),
                Link = URL,
            };

            if (_context.Mini.Any(m => m.Link == mini.Link))
            {
                return _context.Mini.First(m => m.Link == mini.Link);
            }

            return null;
        }

        //TODO - Patreon currently disabled due to thumbnail expiring. Need to add caching of thumbnails somewhere to fix it.
        private async Task<Mini> ParsePatreon(string URL)
        {
            using (HttpClient client = new HttpClient())
            {
                string[] SplitURL = URL.Split('/', '-');

                HttpResponseMessage response = await client.GetAsync("https://www.patreon.com/api/posts/" + SplitURL.Last());
                HttpContent responseContent = response.Content;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                    {
                        string result = await reader.ReadToEndAsync();
                        JObject currentMini = JsonConvert.DeserializeObject<JObject>(result);

                        Creator creator = new Creator
                        {
                            PatreonURL = currentMini["included"][0]["attributes"]["url"].ToString()
                        };

                        Mini mini = new Mini
                        {
                            Creator = creator,
                            Name = currentMini["data"]["attributes"]["title"].ToString(),
                            Link = currentMini["data"]["attributes"]["url"].ToString(),
                            Thumbnail = currentMini["data"]["attributes"]["image"]["large_url"].ToString(),
                            Cost = 1
                        };

                        if (_context.Mini.Any(m => m.Link == mini.Link))
                        {
                            return _context.Mini.First(m => m.Link == mini.Link);
                        }
                    }
                }
                return null;
            }
        }
    }
}