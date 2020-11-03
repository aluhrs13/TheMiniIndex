using System;

namespace MiniIndex.Models.SourceSites
{
    public class AtelierStoriaSource : SourceSite
    {
        public AtelierStoriaSource(Creator creator)
            : base(creator)
        {
            CreatorUserName = "atelierstoria";
        }

        protected AtelierStoriaSource()
        {
        }

        public override Uri BaseUri => new Uri("https://atelierstoria.com/");
        public override Uri CreatorPageUri => BaseUri;
    }
}
