using Microsoft.EntityFrameworkCore;
using SocialService.Application.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Infrastructure.Data;


namespace SocialService.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly SocialDbContext _context;

        public PostRepository(SocialDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetByIdAsync(Guid id)
        {
            return await _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Post>> GetByVenueIdAsync(Guid venueId)
        {
            return await _context.Posts
                .Where(p => p.VenueId == venueId)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Photos)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }


        public async Task DeletePostsByVenueIdAsync(Guid venueId)
        {
            var posts = await _context.Posts
                .Where(p => p.VenueId == venueId)
                .ToListAsync();

            _context.Posts.RemoveRange(posts);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostsByUserIdAsync(Guid userId)
        {
            var posts = await _context.Posts
                .Where(p => p.UserId == userId)
                .ToListAsync();

            _context.Posts.RemoveRange(posts);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAuthorDetailsAsync(Guid userId, string newUserName, string? newAvatarUrl)
        {
            await _context.Posts
                .Where(p => p.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.AuthorUserName, newUserName)
                    .SetProperty(p => p.AuthorAvatarUrl, newAvatarUrl));
        }

        public async Task UpdateVenueDetailsAsync(Guid venueId, string newVenueName, string? newVenueImageUrl)
        {
            await _context.Posts
                .Where(p => p.VenueId == venueId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.VenueName, newVenueName)
                    .SetProperty(p => p.VenueImageUrl, newVenueImageUrl));
        }




        // PostRepository sınıfının içine:

        public async Task DeleteAsync(Guid id)
        {
            // 1. Önce silinecek postu bul
            var post = await _context.Posts.FindAsync(id);

            // 2. Eğer post varsa sil
            if (post != null)
            {
                _context.Posts.Remove(post);

                // 3. Değişiklikleri kaydet
                await _context.SaveChangesAsync();
            }
            
        }


        

        public async Task<List<Post>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Photos)
                .OrderByDescending(p => p.CreatedDate) // En yeni postlar en üstte görünsün
                .ToListAsync();
        }


        public async Task<List<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .Include(p => p.Photos)
                .OrderByDescending(p => p.CreatedDate) // En yeniler en üstte
                .ToListAsync();
        }


        // Kullanıcının bu postu daha önce kaydedip kaydetmediğini kontrol eder
        public async Task<PostSaved> GetByPostAndUserAsync(Guid postId, Guid userId)
        {
            return await _context.PostSaveds
                .FirstOrDefaultAsync(s => s.PostId == postId && s.UserId == userId);
        }

        // Kaydedilen postu favorilerden çıkarır
        public async Task RemoveAsync(PostSaved postSaved)
        {
            _context.PostSaveds.Remove(postSaved);
            await _context.SaveChangesAsync();
        }

    }
}