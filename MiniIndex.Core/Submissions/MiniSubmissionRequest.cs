using MediatR;
using Microsoft.AspNetCore.Identity;
using MiniIndex.Models;
using System;

namespace MiniIndex.Core.Submissions
{
    public class MiniSubmissionRequest : IRequest<Mini>
    {
        public MiniSubmissionRequest(Uri url, IdentityUser user)

        {
            Url = url;
            User = user;
        }

        public Uri Url { get; }
        public IdentityUser User { get; }
    }
}