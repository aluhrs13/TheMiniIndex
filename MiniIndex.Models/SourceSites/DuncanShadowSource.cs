using System;

namespace MiniIndex.Models.SourceSites
{
    public class DuncanShadowSource : SourceSite
    {
        public DuncanShadowSource(Creator creator)
            : base(creator)
        {
            CreatorUserName = "DuncanShadow";
        }

        protected DuncanShadowSource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.duncanshadow.com/");
        public override Uri CreatorPageUri => BaseUri;
    }
}
