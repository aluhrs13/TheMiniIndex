using System;

namespace MiniIndex.Models
{
    public abstract class SourceSite : IEntity
    {
        protected SourceSite(Creator creator)
        {
            Creator = creator;
        }

        protected SourceSite()
        {
        }

        public int ID { get; set; }

        public string SiteName { get; protected set; }
        public abstract Uri BaseUri { get; }
        public virtual string DisplayName => SiteName;

        public Creator Creator { get; set; }

        public string CreatorUserName { get; set; }
        public abstract Uri CreatorPageUri { get; }
    }
}