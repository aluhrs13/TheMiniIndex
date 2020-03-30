using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
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
        [BindProperty(SupportsGet = true)]
        public string Redirect { get; set; }

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

                Mini = await _context.Mini
                                .Include(m=>m.MiniTags)
                                .FirstOrDefaultAsync(m => m.ID == MiniID);

                if (Mini == null)
                {
                    return NotFound();
                }

                if (NewStatus == "Approved")
                {
                    Mini.Status = Status.Approved;
                    Mini.ApprovedTime = DateTime.Now;
                    foreach(MiniTag mt in Mini.MiniTags)
                    {
                        if(mt.Status == Status.Pending)
                        {
                            mt.Status = Status.Approved;
                            mt.ApprovedTime = DateTime.Now;
                            mt.LastModifiedTime = DateTime.Now;
                            _context.Attach(mt).State = EntityState.Modified;
                        }
                    }
                    _context.Attach(Mini).State = EntityState.Modified;
                }

                if (NewStatus == "Pending")
                {
                    Mini.Status = Status.Pending;
                    _context.Attach(Mini).State = EntityState.Modified;
                }

                if (NewStatus == "Rejected")
                {
                    Mini.Status = Status.Rejected;
                    _context.Attach(Mini).State = EntityState.Modified;
                }

                if (NewStatus == "Deleted")
                {
                    Mini.Status = Status.Deleted;
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

                if (Redirect == "Admin")
                {
                    return RedirectToPage("/Admin/Index");
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