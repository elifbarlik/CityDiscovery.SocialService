using SocialService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Application.Interfaces
{
    public interface IPostSavedRepository
    {
        Task AddAsync(PostSaved postSaved);
        Task<PostSaved> GetByPostAndUserAsync(Guid postId, Guid userId);
        Task RemoveAsync(PostSaved postSaved);
    }
}