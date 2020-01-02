using System;

namespace MiniIndex.Models.SourceSites
{
    public class MiniSourceSite : IEntity
    {
        public MiniSourceSite(Mini mini, SourceSite site, Uri link)
        {
            Mini = mini;
            Site = site;
            Link = link;
        }

        protected MiniSourceSite()
        {
        }

        public int ID { get; }

        public Mini Mini { get; set; }
        public SourceSite Site { get; set; }
        public Uri Link { get; set; }
    }
}