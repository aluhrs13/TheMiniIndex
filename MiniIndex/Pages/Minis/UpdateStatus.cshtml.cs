using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.Minis
{
    public class UpdateStatusModel : PageModel
    {
        public UpdateStatusModel(MiniIndexContext context)
        {
            _context = context;
        }

        private readonly MiniIndexContext _context;

        [BindProperty(SupportsGet = true)]
        public int? MiniID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string NewStatus { get; set; }

        [BindProperty]
        public Mini Mini { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                if (MiniID == null || NewStatus == null)
                {
                    return NotFound();
                }

                Mini = await _context.Mini.FirstOrDefaultAsync(m => m.ID == MiniID);

                if (Mini == null)
                {
                    return NotFound();
                }

                if (NewStatus == "Approved")
                {
                    Mini.Status = Status.Approved;
                    _context.Attach(Mini).State = EntityState.Modified; //TODO: is this needed? Change tracking should catch this...
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