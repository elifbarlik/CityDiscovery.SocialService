using CityDiscovery.SocialService.SocialServiceShared.Common.DTOs.Social;
using MediatR;
using SocialService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SocialService.Application.Queries.GetAllPosts
{
    public class GetAllPostsHandler : IRequestHandler<GetAllPostsQuery, List<PostDto>>
    {
        private readonly IPostRepository _postRepository;

        public GetAllPostsHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
        {
            var posts = await _postRepository.GetAllAsync();

            return posts.Select(post => new PostDto
            {
                Id = post.Id,
                VenueId = post.VenueId,
                AuthorUserId = post.UserId,
                AuthorUserName = post.AuthorUserName,
                AuthorAvatarUrl = post.AuthorAvatarUrl,
                Caption = post.Content,
                PhotoUrls = post.Photos?.Select(p => p.ImageUrl).ToList() ?? new List<string>(),
                LikeCount = post.Likes?.Count ?? 0,
                CommentCount = post.Comments?.Count ?? 0,
                CreatedAt = post.CreatedDate,
                VenueName = post.VenueName,
                VenueImageUrl = post.VenueImageUrl
            }).ToList();
        }
    }
}