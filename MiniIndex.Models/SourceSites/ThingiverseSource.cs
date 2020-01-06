using System;

namespace MiniIndex.Models.SourceSites
{
    public class ThingiverseSource : SourceSite
    {
        public ThingiverseSource(Creator creator, string thingiverseUsername)
            : base(creator)
        {
            CreatorUserName = thingiverseUsername;
        }

        protected ThingiverseSource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.thingiverse.com");

        public override Uri CreatorPageUri => new Uri(BaseUri, CreatorUserName);
    }
}