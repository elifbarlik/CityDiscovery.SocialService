using SocialService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Application.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAsync(PostComment comment);
        Task<List<PostComment>> GetByPostIdAsync(Guid postId);
        Task<PostComment> GetByIdAsync(Guid id);
        Task UpdateAuthorDetailsAsync(Guid userId, string newUserName, string? newAvatarUrl);
        Task DeleteAsync(Guid id);
    }
}