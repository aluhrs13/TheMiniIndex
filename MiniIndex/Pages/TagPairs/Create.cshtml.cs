using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.TagPairs
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public CreateModel(
                UserManager<IdentityUser> userManager,
                MiniIndexContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public async Task<IActionResult> OnGet(int tag1, int tag2, int type)
        {

            if (User.IsInRole("Moderator"))
            {
                Tag Tag1 = _context.Tag.FirstOrDefault(t => t.ID == tag1);
                Tag Tag2 = _context.Tag.FirstOrDefault(t => t.ID == tag2);
                PairType pairType = (PairType)type;

                //TODO - This is a hack. 99 means "child" from the tag manager, so we're swapping them.
                //It's a dumb hack, but I'm tired.
                if (type == 99)
                {
                    Tag1 = _context.Tag.FirstOrDefault(t => t.ID == tag2);
                    Tag2 = _context.Tag.FirstOrDefault(t => t.ID == tag1);
                    pairType = PairType.Parent;
                }

                if(Tag1 == null | Tag2 == null)
                {
                    return NotFound();
                }

                TagPair newPair = new TagPair(){
                    Tag1 = Tag1,
                    Tag2 = Tag2,
                    Type = pairType
                };

                if(_context.TagPair.Any(tp=> (tp.Tag1==Tag1 && tp.Tag2 == Tag2)))
                {
                    return NotFound();
                }

                _context.TagPair.Add(newPair);

                await _context.SaveChangesAsync();
                return Page();
            }

            return NotFound();
        }
    }
}