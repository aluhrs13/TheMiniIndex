using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Pages.Admin
{
    [Authorize]

    public class _404FinderModel : PageModel
    {
        private readonly MiniIndex.Models.MiniIndexContext _context;
        public IList<Mini> Mini { get; set; }
        public List<Mini> MissingMinis { get; set; }

        public _404FinderModel(MiniIndex.Models.MiniIndexContext context)
        {
            _context = context;
        }
        public async Task OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                Mini = await _context.Mini
                        .Where(m=>m.Status==Status.Approved)
                        .AsNoTracking()
                        .ToListAsync();

                MissingMinis = new List<Mini>();

                var client = new HttpClient();

                foreach (Mini item in Mini)
                {
                    HttpResponseMessage response = await client.GetAsync(item.Thumbnail);
                    HttpContent responseContent = response.Content;
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        MissingMinis.Add(item);
                    }
                }
            }
        }
    }


}
