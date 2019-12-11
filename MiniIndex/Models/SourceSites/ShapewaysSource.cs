﻿using System;

namespace MiniIndex.Models.SourceSites
{
    public class ShapewaysSource : SourceSite
    {
        public ShapewaysSource(Creator creator, string shapewaysUsername)
            : base(creator)
        {
            ShapewaysUsername = shapewaysUsername;
        }

        protected ShapewaysSource()
        {
        }

        public override Uri BaseUri => new Uri("https://www.shapeways.com/designer/");
        public override Uri CreatorPageUri => new Uri(BaseUri, ShapewaysUsername);

        public string ShapewaysUsername { get; protected set; }
    }
}