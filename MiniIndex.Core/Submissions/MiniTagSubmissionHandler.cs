using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniIndex.Core.Minis.Parsers;
using MiniIndex.Core.Utilities;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MiniIndex.Core.Submissions
{
    public class MiniTagSubmissionHandler : IRequestHandler<MiniTagSubmissionRequest, MiniTag>
    {
        public MiniTagSubmissionHandler(MiniIndexContext context)
        {
            _context = context;
        }
        private readonly MiniIndexContext _context;

        public async Task<MiniTag> Handle(MiniTagSubmissionRequest request, CancellationToken cancellationToken)
        {
            Tag newTag = new Tag();

            //MiniTags can be created by passing a tagName or an ID.
            if (!String.IsNullOrEmpty(request.Tag.TagName))
            {
                newTag = await AddAndFindTagByName(request.Tag.TagName);
            }
            else
            {
                //Don't need to make a new tag, select the existing tag
                newTag = _context.Tag.Where(t => t.ID == request.Tag.ID)
                                .Include(m => m.MiniTags)
                                .First();
            }

            Mini newMini = _context.Mini.Where(m => m.ID == request.Mini.ID)
                            .Include(m => m.Creator)
                            .Include(m => m.MiniTags)
                                .ThenInclude(mt => mt.Tag)
                            .First();

            MiniTag ret = await AddMiniTag(newMini, newTag, request.User);

            //TODO - If this breaks because the tag structure is too complicated serialize the adding of MiniTags by moving the SaveChanges into the function and await each AddMiniTag.
            foreach (Tag pairedTag in FindPairedTags(newTag))
            {
                AddMiniTag(newMini, pairedTag, request.User);
            }
            await _context.SaveChangesAsync();

            //TODO: Change to created
            return ret;

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
                //TODO: Can this be re-used to prevent the select query below
                _context.Tag.Add(newNewTag);
                await _context.SaveChangesAsync();
            }

            return _context.Tag.Where(t => t.TagName == tagName)
                        .Include(m => m.MiniTags)
                        .First();
        }

        public async Task<MiniTag> AddMiniTag(Mini mini, Tag tag, IdentityUser user)
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

                if (newMiniTag.Status == Status.Pending || newMiniTag.Status == Status.Approved)
                {
                    return newMiniTag;
                }
                newMiniTag.Status = Status.Pending;
            }

            foreach (Tag pairedTag in FindPairedTags(tag))
            {
                AddMiniTag(mini, pairedTag, user);
            }

            return newMiniTag;
        }

        public IList<Tag> FindPairedTags(Tag seedTag)
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
    }
}

