using System;

namespace MiniIndex.Models.SourceSites
{
    public class PatreonSource : SourceSite
    {
        public PatreonSource(Creator creator, string patreonUsername)
            : base(creator)
        {
            CreatorUserName = patreonUsername;
        }

        protected PatreonSource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.patreon.com/");
        public override Uri CreatorPageUri => new Uri(BaseUri, CreatorUserName);
    }
}