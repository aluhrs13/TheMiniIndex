using MediatR;
using MiniIndex.Models;

namespace MiniIndex.Core.Submissions
{
    public class MiniSubmissionRequest : IRequest<Mini>
    {
        public MiniSubmissionRequest(string url)
        {
            Url = url;
        }

        public string Url { get; }
    }
}