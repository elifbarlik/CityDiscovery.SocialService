using Microsoft.EntityFrameworkCore;
using SocialService.Application.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialService.Infrastructure.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly SocialDbContext _context;

        public LikeRepository(SocialDbContext context)
        {
            _context = context;
        }
        
        public async Task AddAsync(PostLike like)
        {
            await _context.PostLikes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid postId, Guid userId)
        {
            return await _context.PostLikes
                .AnyAsync(l => l.PostId == postId && l.UserId == userId);
        }

        public async Task<PostLike> GetByPostIdAndUserIdAsync(Guid postId, Guid userId)
        {
            return await _context.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
        }

        public async Task RemoveAsync(PostLike like)
        {
            _context.PostLikes.Remove(like);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountByPostIdAsync(Guid postId)
        {
            return await _context.PostLikes
                .CountAsync(l => l.PostId == postId);
        }
        public async Task<List<PostLike>> GetByPostIdAsync(Guid postId)
        {
            return await _context.PostLikes
                .Where(l => l.PostId == postId)
                .OrderByDescending(l => l.LikedDate) // En son be?enenler en üstte görünsün
                .ToListAsync();
        }
    }
}