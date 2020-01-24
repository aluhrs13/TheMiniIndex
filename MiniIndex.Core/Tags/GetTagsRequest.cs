using MediatR;
using MiniIndex.Models;
using System.Collections.Generic;

namespace MiniIndex.Core.Tags
{
    public class GetTagsRequest : IRequest<IEnumerable<string>>
    {
    }
}