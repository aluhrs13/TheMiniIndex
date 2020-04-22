using Microsoft.AspNetCore.Identity;
using System;

namespace MiniIndex.Models
{
    public class MiniTag
    {
        public int MiniID { get; set; }
        public int TagID { get; set; }
        public Mini Mini { get; set; }
        public Tag Tag { get; set; }
        public Status Status { get; set; }
        public IdentityUser Tagger { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}
