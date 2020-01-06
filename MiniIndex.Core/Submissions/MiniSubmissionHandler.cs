using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

            IParser parser = _parsers.FirstOrDefault(p => p.CanParse(uri));

            if (parser is null)
            {
                //valid URL, but not currently supported
                //TODO: log when this happens?
                return null;
            }

            mini = await parser.ParseFromUrl(uri);
            mini.User = request.User;

            mini.Creator = await GetCreator(mini.Creator, cancellationToken);

            _context.Add(mini);
            await _context.SaveChangesAsync();

            return mini;
        }

        private async Task<Creator> GetCreator(Creator creator, CancellationToken cancellationToken)
        {
            Creator foundCreator = await _context.Set<Creator>()
                .Include(c => c.Sites)
                .FirstOrDefaultAsync(c => c.Name == creator.Name, cancellationToken);

            if (foundCreator is null)
            {
                return creator;
            }

            SourceSite currentSource = creator.Sites.Single();

            if (!foundCreator.Sites.Any(s => s.SiteName == currentSource.SiteName))
            {
                foundCreator.Sites.Add(currentSource);
            }

            return foundCreator;
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