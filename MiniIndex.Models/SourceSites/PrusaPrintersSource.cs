using System;

namespace MiniIndex.Models.SourceSites
{
    public class PrusaPrintersSource : SourceSite
    {
        public PrusaPrintersSource(Creator creator, string prusaprintersUsername)
            : base(creator)
        {
            CreatorUserName = prusaprintersUsername;
        }

        protected PrusaPrintersSource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.shapeways.com/social/");
        public override Uri CreatorPageUri => new Uri(BaseUri, CreatorUserName);
    }
}