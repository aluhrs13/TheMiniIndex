using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Pages.Minis
{                                           
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MiniIndex.Models.MiniIndexContext _context;
        [BindProperty(SupportsGet = true)]
        public string[] SearchString { get; set; }
        public SelectList TagsList { get; set; }
        public PaginatedList<Mini> Mini { get; set; }
        public List<Tag> Tags { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool FreeOnly { get; set; }

        public IndexModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager, 
                MiniIndex.Models.MiniIndexContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task OnGetAsync(int? pageIndex)
        {
            var user = await _userManager.GetUserAsync(User);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            
            var minis = from m in _context.Mini select m;

            if (FreeOnly)
            {
                minis = minis.Where(m => m.Cost == 0);
            }

            if (SearchString!=null && SearchString.Count()>0)
            {
                foreach(string IndividualTag in SearchString)
                {
                    minis=minis.Where(t => t.MiniTags.Any(mt=> mt.Tag.TagName== IndividualTag));
                }
            }
            else
            {
                List<string> SearchList = new List<string>();

                foreach (var key in HttpContext.Request.Query)
                {
                    if (key.Key.Contains("SearchString"))
                    {
                        SearchList.Add(key.Value);
                        string IndividualTag = key.Value;
                        minis = minis.Where(t => t.MiniTags.Any(mt => mt.Tag.TagName == IndividualTag));
                    }
                }

                SearchString = SearchList.ToArray();
            }
            
            //If the user is logged in, we should show them their submitted minis too even if they aren't approved.
            int pageSize = 21;
            if (user != null)
            {
                Mini = await PaginatedList<Mini>
                    .CreateAsync(minis
                    .Include(m => m.MiniTags)
                        .ThenInclude(mt => mt.Tag)
                    .Include(m => m.Creator)
                    .Where(m=>m.Status==Status.Approved||m.User==user)
                    .OrderByDescending(m=>m.ID)
                    .AsNoTracking(),
                    pageIndex ?? 1, pageSize);
            }
            else
            {
                Mini = await PaginatedList<Mini>
                    .CreateAsync(minis
                        .Include(m => m.MiniTags)
                        .ThenInclude(mt => mt.Tag)
                        .Include(m => m.Creator)
                        .Where(m => m.Status == Status.Approved)
                        .OrderByDescending(m => m.ID)
                        .AsNoTracking(),
                        pageIndex ?? 1, pageSize);
            }

            //Pull together tag list for the search box
            IQueryable<Tag> tagsQuery = from m in _context.Tag
                                              orderby m.TagName
                                              select m;

            TagsList = new SelectList(await tagsQuery.Distinct().ToListAsync(),"TagName","TagName",null,"Category");

            Tags = _context
                        .Tag
                        .AsEnumerable()
                        .OrderBy(m => m.Category.ToString())
                        .ThenBy(m => m.TagName)
                        .ToList();
        }
    }
}
