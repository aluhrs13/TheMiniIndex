using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Minis
{
    public class UpdateStatusModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MiniIndexContext _context;
        [BindProperty(SupportsGet = true)]
        public int MiniID { get; set; }
        [BindProperty(SupportsGet = true)]
        public string NewStatus { get; set; }
        [BindProperty]
        public Mini Mini { get; set; }

        public UpdateStatusModel(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        MiniIndexContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                if (MiniID == null || NewStatus==null)
                {
                    return NotFound();
                }

                Mini = await _context.Mini
                    .FirstOrDefaultAsync(m => m.ID == MiniID);

                if (Mini == null)
                {
                    return NotFound();
                }

                if (NewStatus == "Approved")
                {
                    Mini.Status = Status.Approved;
                    _context.Attach(Mini).State = EntityState.Modified;
                }

                if (NewStatus == "Rejected")
                {
                    Mini.Status = Status.Rejected;
                    _context.Attach(Mini).State = EntityState.Modified;
                }


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

                return RedirectToPage("./Details", new { id = Mini.ID });
            }
            else
            {
                return NotFound();
            }


        }
        private bool MiniExists(int id)
        {
            return _context.Mini.Any(e => e.ID == id);
        }
    }
}
