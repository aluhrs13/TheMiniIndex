using MediatR;
using Microsoft.AspNetCore.Identity;
using MiniIndex.Models;
using System;

namespace MiniIndex.Core.Submissions
{
    public class MiniSubmissionRequest : IRequest<Mini>
    {
        public MiniSubmissionRequest(Uri url, IdentityUser user, bool justThumbnail)

        {
            Url = url;
            User = user;
            JustThumbnail = justThumbnail;
        }

        public Uri Url { get; }
        public IdentityUser User { get; }
        public bool JustThumbnail { get; }
    }
}