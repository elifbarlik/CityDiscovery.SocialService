using MediatR;
using SocialService.Application.DTOs;
using SocialService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SocialService.Application.Queries.GetPostLikes
{
    public class GetPostLikesHandler : IRequestHandler<GetPostLikesQuery, List<LikeDto>>
    {
        private readonly ILikeRepository _likeRepository;

        public GetPostLikesHandler(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public async Task<List<LikeDto>> Handle(GetPostLikesQuery request, CancellationToken cancellationToken)
        {
            var likes = await _likeRepository.GetByPostIdAsync(request.PostId);

            return likes.Select(l => new LikeDto
            {
                Id = l.Id,
                PostId = l.PostId,
                UserId = l.UserId,
                LikedDate = l.LikedDate
            }).ToList();
        }
    }
}