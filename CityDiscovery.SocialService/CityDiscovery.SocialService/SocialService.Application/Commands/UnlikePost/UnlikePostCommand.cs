using MediatR;
using System;

namespace SocialService.Application.Commands.UnlikePost
{
    public class UnlikePostCommand : IRequest<bool>
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}