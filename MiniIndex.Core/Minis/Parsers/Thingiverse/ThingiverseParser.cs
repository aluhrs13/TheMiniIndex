using MiniIndex.Models;
using MiniIndex.Models.SourceSites;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis.Parsers.Thingiverse
{
    public class ThingiverseParser : IParser
    {
        public ThingiverseParser(ThingiverseClient thingiverseClient)
        {
            _thingiverseClient = thingiverseClient;
        }

        private readonly ThingiverseClient _thingiverseClient;

        public string Site => "Thingiverse";

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
                Name = thing.creator.name
            };

            var source = new ThingiverseSource(creator, thing.creator.public_url);
            creator.Sites.Add(source);

            var mini = new Mini
            {
                Name = thing.name,
                Status = Status.Pending,
                Cost = 0,
                Link = thing.url,
                Thumbnail = thing.default_image.sizes.FirstOrDefault(i => i.type == "preview" && i.size == "featured").url,
                Creator = creator
            };
            mini.Sources.Add(new MiniSourceSite(mini, source, url));

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