using System;

namespace MiniIndex.Models.SourceSites
{
    public class MyMiniFactorySource : SourceSite
    {
        public MyMiniFactorySource(Creator creator, string myMiniFactoryUsername)
            : base(creator)
        {
            MyMiniFactoryUsername = myMiniFactoryUsername;
        }

        protected MyMiniFactorySource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.myminifactory.com/users/");
        public override Uri CreatorPageUri => new Uri(BaseUri, MyMiniFactoryUsername);
        public string MyMiniFactoryUsername { get; protected set; }
    }
}