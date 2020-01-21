using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Minis.Parsers;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniIndex.Core.Submissions
{
    public class MiniSubmissionHandler : IRequestHandler<MiniSubmissionRequest, Mini>
    {
        public MiniSubmissionHandler(MiniIndexContext context, IEnumerable<IParser> parsers)
        {
            _context = context;
            _parsers = parsers;
        }

        private readonly MiniIndexContext _context;
        private readonly IEnumerable<IParser> _parsers;

        public async Task<Mini> Handle(MiniSubmissionRequest request, CancellationToken cancellationToken)
        {
            Mini mini = await _context.Set<Mini>().SingleOrDefaultAsync(m => m.Link == request.Url.ToString(), cancellationToken);

            if (mini != null)
            {
                return mini;
            }

            IParser parser = _parsers.FirstOrDefault(p => p.CanParse(request.Url));

            if (parser is null)
            {
                //valid URL, but not currently supported
                //TODO: log when this happens?
                return null;
            }

            mini = await parser.ParseFromUrl(request.Url);
            mini.User = request.User;

            _context.Add(mini);

            await CorrectMiniCreator(mini, cancellationToken);

            await _context.SaveChangesAsync();

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
                mini.Creator = foundCreator;
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
    }
}