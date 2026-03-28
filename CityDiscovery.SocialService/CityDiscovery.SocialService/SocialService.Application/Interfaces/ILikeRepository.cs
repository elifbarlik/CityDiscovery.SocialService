using SocialService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Application.Interfaces
{
    public interface ILikeRepository
    {
        Task AddAsync(PostLike like);
        Task<bool> ExistsAsync(Guid postId, Guid userId);
        Task RemoveAsync(PostLike like);
        Task<PostLike> GetByPostIdAndUserIdAsync(Guid postId, Guid userId);
        Task<int> GetCountByPostIdAsync(Guid postId);

        Task<List<PostLike>> GetByPostIdAsync(Guid postId);
    }
}