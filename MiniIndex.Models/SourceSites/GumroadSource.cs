using System;

namespace MiniIndex.Models.SourceSites
{
    public class GumroadSource : SourceSite
    {
        public GumroadSource(Creator creator, string gumroadUsername)
        {
            Creator = creator;

            GumroadUsername = gumroadUsername;
        }

        protected GumroadSource()
        {

        }

        public override Uri BaseUri => new Uri("https://gumroad.com");
        public override Uri CreatorPageUri => new Uri(BaseUri, GumroadUsername);
        
        public string GumroadUsername { get; set; }
    }
}