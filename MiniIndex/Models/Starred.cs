using Microsoft.AspNetCore.Identity;

namespace MiniIndex.Models
{
    public class Starred
    {
        public int MiniID { get; set; }
        public Mini Mini { get; set; }
        public string UserID { get; set; }
        public IdentityUser User{ get; set; }
    }
}
