using System;

namespace MiniIndex.Models.SourceSites
{
    public class WebsiteSource : SourceSite
    {
        public WebsiteSource(Creator creator, Uri websiteUri, Uri profilePageUri = null)
            : base(creator)
        {
            WebsiteURL = websiteUri;
            WebsiteProfilePageURL = profilePageUri;
        }

        protected WebsiteSource()
        {
        }

        public override string DisplayName => "Website";

        public override Uri BaseUri => WebsiteURL;
        public override Uri CreatorPageUri => WebsiteProfilePageURL ?? WebsiteURL;

        public Uri WebsiteURL { get; protected set; }
        public Uri WebsiteProfilePageURL { get; protected set; }
    }
}