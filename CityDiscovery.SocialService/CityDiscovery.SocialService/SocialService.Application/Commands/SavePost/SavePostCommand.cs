using MediatR;
using System;

namespace CityDiscovery.SocialService.SocialService.Application.Commands.SavePost
{
    public class SavePostCommand : IRequest<bool>
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}