using MediatR;
using System;

namespace SocialService.Application.Commands.DeleteComment
{
    public class DeleteCommentCommand : IRequest<bool>
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; } // Güvenlik kontrolü için isteği atan kişi
    }
}