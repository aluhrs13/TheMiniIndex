﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;
using MiniIndex.Persistence;

namespace MiniIndex.Pages.Admin
{
    [Authorize]
    public class ThumbnailCorrectionModel : PageModel
    {
        private readonly MiniIndexContext _context;
        public IList<Mini> Mini { get; set; }

        public ThumbnailCorrectionModel(MiniIndexContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            if (User.IsInRole("Moderator"))
            {
                Mini = await _context.Mini
                    .Where(m => m.Thumbnail.Contains("medium_thumb"))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }
}