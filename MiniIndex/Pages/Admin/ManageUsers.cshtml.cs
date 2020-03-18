using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.ManageUsers
{
    [Authorize]
    public class ManageUsersModel : PageModel
    {
        private readonly MiniIndexContext _context;
        public IList<IGrouping<IdentityUser, Mini>> UserMinis { get; set; }
        public IList<IGrouping<IdentityUser, MiniTag>> UserTags { get; set; }


        public ManageUsersModel(MiniIndexContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {

            UserMinis = _context.Mini
                            .Include(m => m.User)
                            .AsEnumerable()
                            .GroupBy(m => m.User)
                            .OrderByDescending(m => m.Count())
                            .ToList();

            UserTags = _context.MiniTag
                .Include(m => m.Tagger)
                .AsEnumerable()
                .GroupBy(m => m.Tagger)
                .ToList();

        }
    }
}
