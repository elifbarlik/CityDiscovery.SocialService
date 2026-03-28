using MediatR;
using System;

namespace SocialService.Application.Commands.UnsavePost
{
    public class UnsavePostCommand : IRequest<bool>
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}