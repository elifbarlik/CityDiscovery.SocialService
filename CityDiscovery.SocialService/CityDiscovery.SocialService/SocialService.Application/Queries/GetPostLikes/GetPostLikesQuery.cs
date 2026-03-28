using MediatR;
using SocialService.Application.DTOs;
using System;
using System.Collections.Generic;

namespace SocialService.Application.Queries.GetPostLikes
{
    public class GetPostLikesQuery : IRequest<List<LikeDto>>
    {
        public Guid PostId { get; set; }
    }
}