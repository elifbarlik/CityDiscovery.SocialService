using Microsoft.EntityFrameworkCore;
using SocialService.Application.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Infrastructure.Data;


namespace SocialService.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly SocialDbContext _context;
        public CommentRepository(SocialDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PostComment comment)
        {
            await _context.PostComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PostComment>> GetByPostIdAsync(Guid postId)
        {
            return await _context.PostComments
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<PostComment> GetByIdAsync(Guid id)
        {
            return await _context.PostComments
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        
        public async Task UpdateAuthorDetailsAsync(Guid userId, string newUserName, string? newAvatarUrl)
        {
            await _context.PostComments
                .Where(c => c.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.AuthorUserName, newUserName)
                    .SetProperty(c => c.AuthorAvatarUrl, newAvatarUrl));
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _context.PostComments.FindAsync(id);
            if (comment != null)
            {
                _context.PostComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }
}