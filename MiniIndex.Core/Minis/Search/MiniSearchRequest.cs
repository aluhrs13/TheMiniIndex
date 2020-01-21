using MediatR;
using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using System.Collections.Generic;

namespace MiniIndex.Core.Minis.Search
{
    public class MiniSearchRequest : IRequest<PaginatedList<Mini>>
    {
        public string SearchString { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}