using MediatR;
using SocialService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocialService.Application.Commands.DeleteComment
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly ICommentRepository _commentRepository;

        public DeleteCommentHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            // 1. Silinecek yorumu getir
            var comment = await _commentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                throw new Exception("Yorum bulunamadı.");
            }

            // 2. Güvenlik Kontrolü: İsteği atan kullanıcı, yorumun sahibi mi?
            if (comment.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Bu yorumu silme yetkiniz bulunmamaktadır.");
            }

            // 3. Yorumu sil
            await _commentRepository.DeleteAsync(request.CommentId);

            return true;
        }
    }
}