using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniIndex.Models
{
    public class Mini : IEntity
    {
        public Mini()
        {
            MiniTags = new List<MiniTag>();
            Sources = new List<MiniSourceSite>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Thumbnail { get; set; }
        public Creator Creator { get; set; }
        public Status Status { get; set; }
        public List<MiniTag> MiniTags { get; set; }
        public List<MiniSourceSite> Sources { get; set; }
        public IdentityUser User { get; set; }
        public int Cost { get; set; }
        public DateTime ApprovedTime { get; set; }

        //https://www.fluxbytes.com/csharp/convert-datetime-to-unix-time-in-c/
        public long ApprovedLinuxTime()
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(ApprovedTime.ToUniversalTime() - sTime).TotalSeconds*1000;
        }
    }
}