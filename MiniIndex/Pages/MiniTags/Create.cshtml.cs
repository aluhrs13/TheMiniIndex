using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.Pages.MiniTags
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public CreateModel(
                IConfiguration configuration,
                UserManager<IdentityUser> userManager,
                MiniIndexContext context)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }

        private readonly IConfiguration _configuration;
        private readonly MiniIndexContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty(SupportsGet = true)]
        public string mini { get; set; }

        [BindProperty(SupportsGet = true)]
        public string tag { get; set; }

        [BindProperty(SupportsGet = true)]
        public string tagName { get; set; }

        [BindProperty]
        public MiniTag MiniTag { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(mini))
            {
                return Page();
            }

            if (String.IsNullOrEmpty(tag) && String.IsNullOrEmpty(tagName))
            {
                return Page();
            }

            IdentityUser user = await _userManager.GetUserAsync(User);

            Tag newTag = new Tag();

            //MiniTags can be created by passing a tagName or an ID.
            if (!String.IsNullOrEmpty(tagName))
            {
                newTag = await AddAndFindTagByName(tagName);
            }
            else
            {
                //Don't need to make a new tag, select the existing tag
                newTag = _context.Tag.Where(t => t.ID == Int32.Parse(tag))
                                .Include(m => m.MiniTags)
                                .First();
            }

            Mini newMini = _context.Mini.Where(m => m.ID == Int32.Parse(mini))
                            .Include(m => m.Creator)
                            .Include(m => m.MiniTags)
                                .ThenInclude(mt => mt.Tag)
                            .First();

            AddMiniTag(newMini, newTag, user);

            //TODO - If this breaks because the tag structure is too complicated serialize the adding of MiniTags by moving the SaveChanges into the function and await each AddMiniTag.
            foreach(Tag pairedTag in FindPairedTags(newTag))
            {
                AddMiniTag(newMini, pairedTag, user);
            }
            await _context.SaveChangesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("./Index");
        }

        public async Task<Tag> AddAndFindTagByName(string tagName)
        {
            Tag newNewTag = new Tag
            {
                TagName = tagName
            };

            //Create the tag
            if (!_context.Tag.Any(m => m.TagName == newNewTag.TagName))
            {
                _context.Tag.Add(newNewTag);
                await _context.SaveChangesAsync();
            }

            return _context.Tag.Where(t => t.TagName == tagName)
                        .Include(m => m.MiniTags)
                        .First();
        }

        public async void AddMiniTag(Mini mini, Tag tag, IdentityUser user)
        {
            MiniTag newMiniTag = new MiniTag()
            {
                Mini = mini,
                MiniID = mini.ID,
                Tag = tag,
                TagID = tag.ID,
                Tagger = user,
                Status = Status.Pending,
                CreatedTime = DateTime.Now,
                LastModifiedTime = DateTime.Now
            };

            if (!mini.MiniTags.Where(mt => mt.Tag.TagName == tag.TagName).Any())
            {
                tag.MiniTags.Add(newMiniTag);
                mini.MiniTags.Add(newMiniTag);
                _context.MiniTag.Add(newMiniTag);
            }
            else
            {
                newMiniTag = mini.MiniTags.Where(mt => mt.Tag.TagName == tag.TagName).First();

                if(newMiniTag.Status == Status.Pending || newMiniTag.Status == Status.Approved)
                {
                    return;
                }
                newMiniTag.Status = Status.Pending;
            }

            foreach (Tag pairedTag in FindPairedTags(tag))
            {
                AddMiniTag(mini, pairedTag, user);
            }
        }

        public IList<Tag> FindPairedTags(Tag seedTag)
        {
            if (_configuration["CreateTagPairs"] != "false")
            {
                IList<Tag> synonyms = _context.TagPair
                            .Include(tp => tp.Tag1)
                            .Include(tp => tp.Tag2)
                        .Where(tp => tp.Type == PairType.Synonym && (tp.Tag1 == seedTag || tp.Tag2 == seedTag))
                        .Select(tp => tp.GetPairedTag(seedTag))
                        .ToList();

                IList<Tag> parents = _context.TagPair
                                            .Include(tp => tp.Tag1)
                                            .Include(tp => tp.Tag2)
                                        .Where(tp => tp.Type == PairType.Parent && tp.Tag1 == seedTag)
                                        .Select(tp => tp.GetPairedTag(tp.Tag1))
                                        .ToList();

                return synonyms.Concat(parents).ToList();
            }
            else
            {
                return new List<Tag>();
            }


        }
    }
}