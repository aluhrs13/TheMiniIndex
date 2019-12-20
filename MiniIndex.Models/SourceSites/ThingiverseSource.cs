using System;

namespace MiniIndex.Models.SourceSites
{
    public class ThingiverseSource : SourceSite
    {
        public ThingiverseSource(Creator creator, string thingiverseUsername)
            : base(creator)
        {
            ThingiverseUsername = thingiverseUsername;
        }

        protected ThingiverseSource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.thingiverse.com");

        public string ThingiverseUsername { get; protected set; }
        public override Uri CreatorPageUri => new Uri(BaseUri, ThingiverseUsername);
    }
}