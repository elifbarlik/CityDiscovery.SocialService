using MediatR;
using SocialService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialService.Application.Commands.UnlikePost
{
    public class UnlikePostHandler : IRequestHandler<UnlikePostCommand, bool>
    {
        private readonly ILikeRepository _likeRepository;

        public UnlikePostHandler(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public async Task<bool> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcının bu gönderideki beğenisini bul
            var existingLike = await _likeRepository.GetByPostIdAndUserIdAsync(request.PostId, request.UserId);

            if (existingLike != null)
            {
                // Beğeni varsa kaldır
                await _likeRepository.RemoveAsync(existingLike);
                return true;
            }

            // Beğeni zaten yoksa false dön
            return false;
        }
    }
}