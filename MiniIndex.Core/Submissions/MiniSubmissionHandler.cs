using MediatR;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Minis;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System;
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
            Uri uri = new Uri(request.Url);

            Mini mini = await _context.Set<Mini>().SingleOrDefaultAsync(m => m.Link == request.Url, cancellationToken);

            if (mini != null)
            {
                return mini;
            }

            IParser parser = _parsers.FirstOrDefault(p => p.CanParse(uri));

            if (parser is null)
            {
                //valid URL, but not currently supported
                //TODO: log when this happens?
                return null;
            }

            mini = await parser.ParseFromUrl(uri);
            mini.User = request.User;

            mini.Creator = await GetCreator(mini.Creator, cancellationToken);

            _context.Add(mini);
            await _context.SaveChangesAsync();

            return mini;
        }

        private async Task<Creator> GetCreator(Creator creator, CancellationToken cancellationToken)
        {
            Creator foundCreator = await _context.Set<Creator>()
                .Include(c => c.Sites)
                .FirstOrDefaultAsync(c => c.Name == creator.Name, cancellationToken);

            if (foundCreator is null)
            {
                return creator;
            }

            SourceSite currentSource = creator.Sites.Single();

            if (!foundCreator.Sites.Any(s => s.SiteName == currentSource.SiteName))
            {
                foundCreator.Sites.Add(currentSource);
            }

            return foundCreator;
        }
    }
}