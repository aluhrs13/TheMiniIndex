using MediatR;
using Microsoft.AspNetCore.Identity;
using MiniIndex.Models;
using System;

namespace MiniIndex.Core.Submissions
{
    public class MiniTagSubmissionRequest : IRequest<MiniTag>
    {
        public MiniTagSubmissionRequest(Mini mini, Tag tag, IdentityUser user)

        {
            Mini = mini;
            Tag = tag;
            User = user;
        }

        public Mini Mini { get; }
        public Tag Tag { get; }
        public IdentityUser User { get; }
    }
}