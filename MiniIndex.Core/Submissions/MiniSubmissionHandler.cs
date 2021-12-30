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

namespace MiniIndex.Core.Submissions
{
    public class MiniSubmissionHandler : IRequestHandler<MiniSubmissionRequest, Mini>
    {
        public MiniSubmissionHandler(MiniIndexContext context, IEnumerable<IParser> parsers, IOptions<AzureStorageConfig> config)
        {
            _context = context;
            _parsers = parsers;
            storageConfig = config.Value;
        }

        private readonly AzureStorageConfig storageConfig = null;
        private readonly MiniIndexContext _context;
        private readonly IEnumerable<IParser> _parsers;

        public async Task<Mini> Handle(MiniSubmissionRequest request, CancellationToken cancellationToken)
        {
            //TODO - This should look at MiniSourceSite, not m.Link.
            Mini mini = await _context.Mini.FirstOrDefaultAsync(m => m.Link == request.Url.ToString(), cancellationToken);
            IParser parser = _parsers.FirstOrDefault(p => p.CanParse(request.Url));

            if (parser is null)
            {
                //valid URL, but not currently supported
                //TODO: log when this happens?
                return null;
            }

            if (mini != null)
            {
                if (request.JustThumbnail)
                {
                    Mini updatedMini = await parser.ParseFromUrl(request.Url);
                    mini.Thumbnail = updatedMini.Thumbnail;
                    _context.Attach(mini).State = EntityState.Modified;
                    await UploadThumbnail(mini);
                }
                return mini;
            }

            mini = await parser.ParseFromUrl(request.Url);

            //TODO - This should look at MiniSourceSite, not m.Link.
            //Now that we've parsed it, check if the parsed URL is different from the original URL and if we have that.
            Mini checkDupe = await _context.Mini.FirstOrDefaultAsync(m => m.Link == mini.Link, cancellationToken);

            if (checkDupe != null)
            {
                if (request.JustThumbnail)
                {
                    checkDupe.Thumbnail = mini.Thumbnail;
                    _context.Attach(mini).State = EntityState.Modified;
                    await UploadThumbnail(checkDupe);
                }
                return checkDupe;
            }

            if (!request.JustThumbnail)
            {
                mini.User = request.User;
                mini.Status = Status.Unindexed;

                _context.Add(mini);

                await CorrectMiniCreator(mini, cancellationToken);

                //TODO - Another dupe check here based on name and creator

                await _context.SaveChangesAsync();
            }

            await UploadThumbnail(mini);

            return mini;
        }

        private async Task CorrectMiniCreator(Mini mini, CancellationToken cancellationToken)
        {
            MiniSourceSite currentSource = mini.Sources.Single();

            //Find a SourceSite that has both the same UserName and SiteName as the Mini's current
            SourceSite matchingSource = await _context.Set<SourceSite>()
                .Include(s => s.Creator).ThenInclude(c => c.Sites)
                .FirstOrDefaultAsync((s => s.CreatorUserName == currentSource.Site.CreatorUserName && s.SiteName == currentSource.Site.SiteName), cancellationToken);

            Creator foundCreator = matchingSource?.Creator;

            if (foundCreator != null)
            {
                _context.Remove(mini.Creator);
                mini.Creator = foundCreator;

                _context.Remove(currentSource.Site);
                currentSource.Site = matchingSource;

                return;
            }

            //If we didn't find a "perfect" match, try matching just off of the creator's names
            foundCreator = await _context.Set<Creator>()
                .Include(c => c.Sites)
                .FirstOrDefaultAsync(c => c.Name == mini.Creator.Name, cancellationToken);

            if (foundCreator != null)
            {
                mini.Creator = foundCreator;

                if (!foundCreator.Sites.Any(s => s.SiteName == currentSource.Site.SiteName))
                {
                    foundCreator.Sites.Add(currentSource.Site);
                }
            }
        }

        private async Task<bool> UploadThumbnail(Mini mini)
        {
            if (!String.IsNullOrEmpty(storageConfig.AccountName))
            {
                string imgURL = mini.Thumbnail;
                string MiniID = mini.ID.ToString();

                if (await StorageHelper.UploadFileToStorage(mini.Thumbnail, MiniID, storageConfig))
                {
                    mini.Thumbnail = "https://" +
                                        storageConfig.AccountName +
                                        ".blob.core.windows.net/" +
                                        storageConfig.ImageContainer +
                                        "/" + MiniID + ".jpg";
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}