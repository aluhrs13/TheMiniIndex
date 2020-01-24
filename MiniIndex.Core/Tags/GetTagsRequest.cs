using MediatR;
using MiniIndex.Models;
using System.Collections.Generic;

namespace MiniIndex.Core.Tags
{
    public class GetTagsRequest : IRequest<IEnumerable<string>>
    {
        public GetTagsRequest(string search)
        {
            SearchTerm = search;
        }

        public string SearchTerm { get; }
    }
}