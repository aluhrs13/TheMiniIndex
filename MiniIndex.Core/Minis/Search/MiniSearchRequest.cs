using MediatR;
using MiniIndex.Core.Pagination;
using MiniIndex.Models;
using System.Collections.Generic;
using System.Linq;

namespace MiniIndex.Core.Minis.Search
{
    public class MiniSearchRequest : IRequest<PaginatedList<Mini>>
    {
        public MiniSearchRequest()
        {
            Tags = Enumerable.Empty<string>();
        }

        private IEnumerable<string> _tags;
        
        public string SearchString { get; set; }
        public IEnumerable<string> Tags 
        { 
            get => _tags; 
            set => _tags = value ?? Enumerable.Empty<string>();
        }
        public bool FreeOnly { get; set; }
        public bool IncludeUnapproved { get; set; }

        public PageInfo PageInfo { get; set; }
    }
}