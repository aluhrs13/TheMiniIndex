using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Admin
{
    [Authorize]
    public class ManageCreatorsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MiniIndexContext _context;
        public List<Creator> Creators { get; set; }
        public List<RecurringJobDto> HangFireJobs { get; set; }

        public ManageCreatorsModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                MiniIndexContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {

                Creators = await _context.Set<Creator>().AsNoTracking().TagWith("Creator Admin List")
                                    .Include(c => c.Sites)
                                    .OrderBy(c=>c.Name)
                                    .ToListAsync();

                List<RecurringJobDto> recurringJobs = new List<RecurringJobDto>();
                HangFireJobs = JobStorage.Current.GetConnection().GetRecurringJobs().ToList();
            }
        }
    }
}
