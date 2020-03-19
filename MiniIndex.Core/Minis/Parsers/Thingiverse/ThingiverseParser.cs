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

            ThingiverseSource source = new ThingiverseSource(creator, thing.creator.public_url);
            creator.Sites.Add(source);

            Mini mini = new Mini
            {
                Name = thing.name,
                Status = Status.Pending,
                Cost = 0,
                Link = thing.public_url,
                Creator = creator
            };

            if (thing.default_image != null)
            {
                mini.Thumbnail = thing.default_image.sizes.FirstOrDefault(i => i.type == "preview" && i.size == "featured").url;
            }
            else
            {
                mini.Thumbnail = thing.thumbnail;
            }

            mini.Sources.Add(new MiniSourceSite(mini, source, url));

            return mini;
        }

        public bool CanParse(Uri url)
        {
            return (url.Host.Replace("www.", "").Equals("thingiverse.com", StringComparison.OrdinalIgnoreCase) && url.ToString().Contains("thing:"));
        }

        private string GetThingIdFromUrl(Uri url)
        {
            return url.AbsolutePath.Split(':').Last();
        }
    }
}