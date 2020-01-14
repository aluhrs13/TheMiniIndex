using System;

namespace MiniIndex.Models.SourceSites
{
    public class MyMiniFactorySource : SourceSite
    {
        public MyMiniFactorySource(Creator creator, string myMiniFactoryUsername)
            : base(creator)
        {
            CreatorUserName = myMiniFactoryUsername;
        }

        protected MyMiniFactorySource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.myminifactory.com/users/");
        public override Uri CreatorPageUri => new Uri(BaseUri, CreatorUserName);
    }
}