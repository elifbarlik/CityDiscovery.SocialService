using SocialService.Domain.Entities;


namespace SocialService.Application.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task<Post> GetByIdAsync(Guid id);
        Task<List<Post>> GetByVenueIdAsync(Guid venueId);
        Task<List<Post>> GetByUserIdAsync(Guid userId);
        Task DeletePostsByVenueIdAsync(Guid venueId);
        Task DeletePostsByUserIdAsync(Guid userId);
        Task UpdateAuthorDetailsAsync(Guid userId, string newUserName, string newAvatarUrl);
        Task UpdateVenueDetailsAsync(Guid venueId, string newVenueName, string newVenueImageUrl);
        Task DeleteAsync(Guid id);
        Task<List<Post>> GetAllAsync();



    }
}