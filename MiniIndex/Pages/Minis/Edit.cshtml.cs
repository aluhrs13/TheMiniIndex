using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    [Authorize]
    public class EditModel : PageModel
    {
        public EditModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                MiniIndexContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MiniIndexContext _context;

        [BindProperty]
        public Mini Mini { get; set; }

        public SelectList CreatorSL { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (User.IsInRole("Moderator"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                Mini = await _context.Mini
                    .Include(m => m.Creator)
                    .Include(m => m.MiniTags)
                        .ThenInclude(mt => mt.Tag)
                    .FirstOrDefaultAsync(m => m.ID == id);

                PopulateCreatorsDropDownList(Mini.Creator.ID);

                if (Mini == null)
                {
                    return NotFound();
                }
                return Page();
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                //TODO - Fix this to enable any status
                //So I can just click submit.
                if (Mini.Status == Status.Pending)
                {
                    Mini.Status = Status.Approved;
                }

                _context.Attach(Mini).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MiniExists(Mini.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToPage("./Admin");
            }
            else
            {
                return NotFound();
            }
        }

        public void PopulateCreatorsDropDownList(object selectedCreator = null)
        {
            IQueryable<Creator> creatorsQuery = from c in _context.Mini
                                                orderby c.Creator.Name
                                                select c.Creator;

            CreatorSL = new SelectList(creatorsQuery.Distinct(), "ID", "Name", selectedCreator);
        }

        private bool MiniExists(int id)
        {
            return _context.Mini.Any(e => e.ID == id);
        }
    }
}