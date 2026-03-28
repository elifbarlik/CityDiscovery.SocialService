using MediatR;
using SocialService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SocialService.Application.Commands.UnsavePost
{
    public class UnsavePostHandler : IRequestHandler<UnsavePostCommand, bool>
    {
        private readonly IPostSavedRepository _postSavedRepository;

        public UnsavePostHandler(IPostSavedRepository postSavedRepository)
        {
            _postSavedRepository = postSavedRepository;
        }

        public async Task<bool> Handle(UnsavePostCommand request, CancellationToken cancellationToken)
        {
            var existingSave = await _postSavedRepository.GetByPostAndUserAsync(request.PostId, request.UserId);

            if (existingSave != null)
            {
                await _postSavedRepository.RemoveAsync(existingSave);
            }

            return true;
        }
    }
}