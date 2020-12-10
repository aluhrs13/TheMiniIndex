using AgileObjects.AgileMapper.Extensions;
using LamarCodeGeneration.Frames;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Core.Tags;
using MiniIndex.Models;
using MiniIndex.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniIndex.API
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : Controller
    {
        public TagsController(MiniIndexContext context, IMediator mediator, TelemetryClient telemetry)
        {
            _context = context;
            _mediator = mediator;
            _telemetry = telemetry;

        }

        private readonly MiniIndexContext _context;
        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetry;


        public async Task<IEnumerable<string>> GetAllTags([FromQuery] string search = null)
        {
            var tags = await _mediator.Send(new GetTagsRequest(search));

            return tags;
        }


        [HttpGet("viewPairs")]
        public async Task<string> Viewpairs()
        {

            string retData = "digraph G{\n";
            retData += "\tlayout=fdp\n";
            IList<Tag> allTags = _context.Set<Tag>()
                                        .Where(t=>t.Category == TagCategory.CreatureName)
                                        .OrderBy(t => t.TagName)
                                        .ToList();

            IEnumerable<string> returnTags = Enumerable.Empty<string>();

            foreach (Tag seedTag in allTags)
            {
                retData += "\t \"" + seedTag.TagName + "\";\n";

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

                foreach (Tag synonym in synonyms)
                {
                    parents = parents.Concat(
                            _context.TagPair
                                .Include(tp => tp.Tag1)
                                .Include(tp => tp.Tag2)
                            .Where(tp => tp.Type == PairType.Parent && tp.Tag1 == seedTag)
                            .Select(tp => tp.GetPairedTag(tp.Tag1))
                        ).ToList();
                }

                returnTags = returnTags.Concat(parents.Where(p=>p.Category!=TagCategory.Location).Distinct().Select(p => "\""+seedTag.TagName + "\" -> \"" + p.TagName+"\""));
            }

            foreach(string retString in returnTags)
            {
                retData += "\t"+retString + ";\n";
            }

            retData += "}";

            return retData;
        }
    }
}