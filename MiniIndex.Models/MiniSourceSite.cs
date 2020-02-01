using System;
using System.Collections.Generic;

namespace MiniIndex.Models
{
    public class MiniSourceSite : IEntity, IDeleteOrphaned
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

        public IEnumerable<object> GetChildren() => new object[] { Site };
    }
}