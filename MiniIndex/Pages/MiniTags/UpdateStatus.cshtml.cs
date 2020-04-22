using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.MiniTags
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
        public int? TagID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string NewStatus { get; set; }


        [BindProperty]
        public MiniTag MiniTag { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                if (TagID == null || MiniID == null || NewStatus == null)
                {
                    return NotFound();
                }

                MiniTag = await _context.MiniTag.FirstOrDefaultAsync(m => m.MiniID == MiniID && m.TagID == TagID);

                if (MiniTag == null)
                {
                    return NotFound();
                }

                if (NewStatus == "Approved")
                {
                    MiniTag.Status = Status.Approved;
                    MiniTag.ApprovedTime = DateTime.Now;
                    MiniTag.LastModifiedTime = DateTime.Now;
                    _context.Attach(MiniTag).State = EntityState.Modified;
                }

                if (NewStatus == "Pending")
                {
                    MiniTag.Status = Status.Pending;
                    MiniTag.LastModifiedTime = DateTime.Now;
                    _context.Attach(MiniTag).State = EntityState.Modified;
                }

                if (NewStatus == "Rejected")
                {
                    MiniTag.Status = Status.Rejected;
                    MiniTag.LastModifiedTime = DateTime.Now;
                    _context.Attach(MiniTag).State = EntityState.Modified;
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }

                return RedirectToPage("/Admin/TagStream");
                
            }
            else
            {
                return NotFound();
            }
        }

    }
}