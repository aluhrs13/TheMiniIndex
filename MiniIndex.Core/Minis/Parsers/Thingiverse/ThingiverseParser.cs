using System;
using System.Linq;
using System.Threading.Tasks;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using MiniIndex.Persistence;

namespace MiniIndex.Core.Minis.Parsers.Thingiverse
{
    internal class ThingiverseParser : IParser
    {
        public ThingiverseParser(ThingiverseClient thingiverseClient, MiniIndexContext persistence)
        {
            _thingiverseClient = thingiverseClient;
            _persistence = persistence;
        }

        private readonly ThingiverseClient _thingiverseClient;
        private readonly MiniIndexContext _persistence;

        public async Task<Mini> ParseFromUrl(Uri url)
        {
            if (!CanParse(url))
            {
                return null;
            }

            string thingId = GetThingIdFromUrl(url);

            ThingiverseModel.Thing thing = await _thingiverseClient.GetThing(thingId);

            if (thing is null)
            {
                return null;
            }

            Creator creator = new Creator
            {
                Name = thing.creator.name,
            };

            creator.Sites.Add(new ThingiverseSource(creator, thing.creator.name));

            Mini mini = new Mini
            {
                Name = thing.name,
                Cost = 0,
                Link = thing.url,
                Thumbnail = thing.thumbnail,
                Status = Status.Pending,
                Creator = creator
            };

            return mini;
        }

        public bool CanParse(Uri url)
        {
            return url.Host.Replace("www.", "").Equals("thingiverse.com", StringComparison.OrdinalIgnoreCase);
        }

        private string GetThingIdFromUrl(Uri url)
        {
            return url.AbsolutePath.Split(':').Last();
        }
    }
}