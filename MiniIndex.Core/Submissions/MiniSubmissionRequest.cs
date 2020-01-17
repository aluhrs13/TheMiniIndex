using MediatR;
using Microsoft.AspNetCore.Identity;
using MiniIndex.Models;

namespace MiniIndex.Core.Submissions
{
    public class MiniSubmissionRequest : IRequest<Mini>
    {
        public MiniSubmissionRequest(string url, IdentityUser user)

        {
            Url = url;
            User = user;
        }

        public string Url { get; }
        public IdentityUser User { get; }
    }
}