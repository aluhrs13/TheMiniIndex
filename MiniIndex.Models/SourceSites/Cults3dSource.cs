using System;

namespace MiniIndex.Models.SourceSites
{
    public class Cults3dSource : SourceSite
    {
        public Cults3dSource(Creator creator, string cults3dUsername)
            : base(creator)
        {
            CreatorUserName = cults3dUsername;
        }

        protected Cults3dSource()
        {
        }

        public override Uri BaseUri => new Uri("https://cults3d.com/en/users/");
        public override Uri CreatorPageUri => new Uri(BaseUri, CreatorUserName);
    }
}
