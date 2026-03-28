using MediatR;
using SocialService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialService.Application.Commands.DeletePost
{
    public class DeletePostHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IPostRepository _postRepository;

        public DeletePostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            // 1. Silinmek istenen postu getir
            var post = await _postRepository.GetByIdAsync(request.PostId);

            if (post == null)
            {
                throw new Exception("Post bulunamadı.");
            }

            // 2. Güvenlik Kontrolü: İsteği atan kullanıcı, postun sahibi mi?
            if (post.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Bu gönderiyi silme yetkiniz bulunmamaktadır.");
            }

            // 3. Postu veritabanından sil (Repository'deki mevcut DeleteAsync metodu)
            // Cascade delete ayarlı olduğu için ilişkili yorumlar, fotoğraflar ve beğeniler otomatik silinecektir.
            await _postRepository.DeleteAsync(request.PostId);

            return true;
        }
    }
}