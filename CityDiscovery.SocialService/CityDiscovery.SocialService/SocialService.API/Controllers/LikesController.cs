using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialService.Application.Commands.LikePost;
using SocialService.Application.Commands.UnlikePost;
using SocialService.Application.DTOs;
using SocialService.Application.Queries.GetPostLikes;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialService.API.Controllers
{
    /// <summary>
    /// Beğeniler API - Gönderilerdeki beğenileri yönetir
    /// </summary>
    [Route("api/posts/{postId}/like")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class LikesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LikesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Bir gönderiyi beğenir veya beğeniyi kaldırır (toggle)
        /// </summary>
        /// <param name="postId">Gönderi ID'si</param>
        /// <returns>Gönderi ID'si, Kullanıcı ID'si ve gönderinin beğenilip beğenilmediği</returns>
        /// <response code="200">Beğeni durumu başarıyla güncellendi</response>
        /// <response code="400">Geçersiz istek veya gönderi bulunamadı</response>
        /// <response code="401">Yetkilendirme hatası (Geçersiz Token)</response>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/posts/{postId}/like
        ///     (Body boş gönderilir, UserId token'dan otomatik alınır)
        ///     
        /// Örnek yanıt (beğenildi):
        /// 
        ///     {
        ///         "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "isLiked": true
        ///     }
        ///     
        /// Örnek yanıt (beğeni kaldırıldı):
        /// 
        ///     {
        ///         "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "isLiked": false
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LikePost(Guid postId) // [FromBody] parametresi tamamen kaldırıldı
        {
            try
            {
                // JWT token'dan UserId'yi alıyoruz
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new { error = "Kullanıcı kimliği doğrulanamadı." });
                }

                // Token'dan aldığımız ID'yi komuta veriyoruz
                var command = new LikePostCommand
                {
                    PostId = postId,
                    UserId = userId
                };

                
                var isLiked = await _mediator.Send(command);

                return Ok(new
                {
                    postId = postId,
                    userId = userId,
                    isLiked = isLiked
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Bir gönderideki beğeniyi kaldırır (Unlike)
        /// </summary>
        /// <param name="postId">Gönderi ID'si</param>
        /// <response code="200">Beğeni başarıyla kaldırıldı</response>
        /// <response code="400">Geçersiz istek</response>
        /// <response code="401">Yetkilendirme hatası (Geçersiz Token)</response>
        [HttpDelete]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnlikePost(Guid postId)
        {
            try
            {
                // JWT token'dan UserId'yi alıyoruz
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new { error = "Kullanıcı kimliği doğrulanamadı." });
                }

                var command = new UnlikePostCommand
                {
                    PostId = postId,
                    UserId = userId
                };

                await _mediator.Send(command);

                return Ok(new
                {
                    postId = postId,
                    userId = userId,
                    isLiked = false,
                    message = "Beğeni kaldırıldı"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Bir gönderiye ait tüm beğenileri (beğenen kullanıcıların ID'leri ile birlikte) getirir
        /// </summary>
        /// <param name="postId">Gönderi ID'si</param>
        /// <returns>Beğeni listesi</returns>
        /// <response code="200">Beğeni listesi başarıyla getirildi</response>
        [HttpGet("~/api/posts/{postId}/likes")]
        [ProducesResponseType(typeof(List<LikeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostLikes(Guid postId)
        {
            try
            {
                var query = new GetPostLikesQuery { PostId = postId };
                var likes = await _mediator.Send(query);

                return Ok(likes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}