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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MiniIndex.Core.Submissions
{
    public class MiniSubmissionHandler : IRequestHandler<MiniSubmissionRequest, Mini>
    {
        public MiniSubmissionHandler(MiniIndexContext context, IEnumerable<IParser> parsers, IOptions<AzureStorageConfig> config, HttpClient httpClient)
        {
            _context = context;
            _parsers = parsers;
            storageConfig = config.Value;
            _httpClient = httpClient;
        }

        private readonly AzureStorageConfig storageConfig = null;
        private readonly MiniIndexContext _context;
        private readonly IEnumerable<IParser> _parsers;
        private readonly HttpClient _httpClient;

        //TODO - This should look at MiniSourceSite, not m.Link.
        //TODO - Log unsupported URLs
        public async Task<Mini> Handle(MiniSubmissionRequest request, CancellationToken cancellationToken)
        {
            string originalLink = request.Url.ToString();
            IParser parser = _parsers.FirstOrDefault(p => p.CanParse(request.Url));

            if (parser is null)
            {
                //valid URL, but not currently supported
                return null;
            }

            //TODO: Merge thumbnail replacement logic
            if (await _context.Mini.AsNoTracking().TagWith("MiniSubmissionHandler.cs 1").AnyAsync(m => m.Link == originalLink, cancellationToken))
            {
                Mini mini = await _context.Mini.TagWith("MiniSubmissionHandler.cs 2").FirstOrDefaultAsync(m => m.Link == originalLink, cancellationToken);

                if (request.JustThumbnail)
                {
                    Mini updatedMini = await parser.ParseFromUrl(request.Url);
                    mini.Thumbnail = updatedMini.Thumbnail;
                    _context.Attach(mini).State = EntityState.Modified;
                    await UploadThumbnail(mini);
                }
                return mini;
            }

            Mini parsedMini = await parser.ParseFromUrl(request.Url);

            //Now that we've parsed it, check if the parsed URL is different from the original URL and if we have that.
            if(parsedMini.Link != originalLink)
            {
                Mini checkDupe = await _context.Mini.TagWith("MiniSubmissionHandler.cs 3").FirstOrDefaultAsync(m => m.Link == parsedMini.Link, cancellationToken);

                if (checkDupe != null)
                {
                    if (request.JustThumbnail)
                    {
                        checkDupe.Thumbnail = parsedMini.Thumbnail;
                        _context.Attach(parsedMini).State = EntityState.Modified;
                        await UploadThumbnail(checkDupe);
                    }
                    return checkDupe;
                }
            }

            if (!request.JustThumbnail)
            {
                parsedMini.User = request.User;
                parsedMini.Status = Status.Unindexed;

                _context.Add(parsedMini);

                await CorrectMiniCreator(parsedMini, cancellationToken);
                await _context.SaveChangesAsync();
            }

            await UploadThumbnail(parsedMini);

            return parsedMini;
        }

        private async Task CorrectMiniCreator(Mini mini, CancellationToken cancellationToken)
        {
            MiniSourceSite currentSource = mini.Sources.Single();

            //Find a SourceSite that has both the same UserName and SiteName as the Mini's current
            SourceSite matchingSource = await _context.Set<SourceSite>().TagWith("MiniSubmissionHandler.cs 4")
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
            foundCreator = await _context.Set<Creator>().TagWith("MiniSubmissionHandler.cs 5")
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

                if (await StorageHelper.UploadFileToStorage(mini.Thumbnail, MiniID, storageConfig, _httpClient))
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