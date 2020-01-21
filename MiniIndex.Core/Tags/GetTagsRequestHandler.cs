using MediatR;
using MiniIndex.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniIndex.Core.Tags
{
    public class GetTagsRequestHandler : IRequestHandler<GetTagsRequest, IEnumerable<Tag>>
    {
        public async Task<IEnumerable<Tag>> Handle(GetTagsRequest request, CancellationToken cancellationToken)
        {
            return Enumerable.Empty<Tag>();
        }
    }
}