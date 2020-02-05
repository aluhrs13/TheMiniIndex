using Microsoft.AspNetCore.Identity;

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
    }
}
