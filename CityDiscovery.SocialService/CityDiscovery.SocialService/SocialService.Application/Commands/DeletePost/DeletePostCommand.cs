using MediatR;
using System;

namespace SocialService.Application.Commands.DeletePost
{
    public class DeletePostCommand : IRequest<bool>
    {
        public Guid PostId { get; set; }

        // Güvenlik için işlemi yapan kişinin ID'sini de alıyoruz
        public Guid UserId { get; set; }
    }
}