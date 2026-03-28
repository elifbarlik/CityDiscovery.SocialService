using MediatR;
using SocialService.Application.Interfaces;
using SocialService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CityDiscovery.SocialService.SocialService.Application.Commands.SavePost
{
    public class SavePostHandler : IRequestHandler<SavePostCommand, bool>
    {
        private readonly IPostSavedRepository _postSavedRepository;
        private readonly IPostRepository _postRepository;

        public SavePostHandler(IPostSavedRepository postSavedRepository, IPostRepository postRepository)
        {
            _postSavedRepository = postSavedRepository;
            _postRepository = postRepository;
        }

        public async Task<bool> Handle(SavePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null) throw new Exception("Gönderi bulunamadı.");

            // Daha önce kaydedilmiş mi kontrol et
            var existingSave = await _postSavedRepository.GetByPostAndUserAsync(request.PostId, request.UserId);
            if (existingSave != null)
            {
                return true; // Zaten kaydedilmiş
            }

            var postSaved = new PostSaved
            {
                PostId = request.PostId,
                UserId = request.UserId,
                SavedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            await _postSavedRepository.AddAsync(postSaved);
            return true;
        }
    }
}
