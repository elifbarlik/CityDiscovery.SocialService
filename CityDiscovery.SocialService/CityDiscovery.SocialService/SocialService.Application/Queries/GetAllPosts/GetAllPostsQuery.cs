using CityDiscovery.SocialService.SocialServiceShared.Common.DTOs.Social;
using MediatR;
using System.Collections.Generic;

namespace SocialService.Application.Queries.GetAllPosts
{
    public class GetAllPostsQuery : IRequest<List<PostDto>>
    {
        // Bu sorgu herhangi bir parametre almadığı için içi boş kalacak.
    }
}