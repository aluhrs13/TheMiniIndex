using System;

namespace MiniIndex.Models.SourceSites
{
    public class Wargaming3dSource : SourceSite
    {
        public Wargaming3dSource(Creator creator, string wargaming3dUsername)
            : base(creator)
        {
            CreatorUserName = wargaming3dUsername;
        }

        protected Wargaming3dSource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.wargaming3d.com/vendor/");
        public override Uri CreatorPageUri => new Uri(BaseUri, CreatorUserName);
    }
}
