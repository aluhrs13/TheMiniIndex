﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MiniIndex.Models
{
    public class Mini : IEntity
    {
        public Mini()
        {
            MiniTags = new List<MiniTag>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Thumbnail { get; set; }
        public Creator Creator { get; set; }
        public Status Status { get; set; }
        public List<MiniTag> MiniTags { get; set; }
        public IdentityUser User { get; set; }
        public int Cost { get; set; }
    }
}