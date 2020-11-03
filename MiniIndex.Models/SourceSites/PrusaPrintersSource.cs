using System;

namespace MiniIndex.Models.SourceSites
{
    public class PrusaPrintersSource : SourceSite
    {

        public PrusaPrintersSource(Creator creator, string prusaid, string prusaprintersUsername)
            : base(creator)
        {
            CreatorUserName = prusaid + "-" + prusaprintersUsername;
        }

        protected PrusaPrintersSource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.prusaprinters.org/social/");
        public override Uri CreatorPageUri => new Uri(BaseUri, CreatorUserName);
    }
}