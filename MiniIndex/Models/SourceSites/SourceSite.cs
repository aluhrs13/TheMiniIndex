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

        public abstract string DisplayName { get; protected set; }
        public abstract Uri BaseUri { get; }

        public abstract Uri CreatorPageUri { get; }

        public Creator Creator { get; set; }
    }
}