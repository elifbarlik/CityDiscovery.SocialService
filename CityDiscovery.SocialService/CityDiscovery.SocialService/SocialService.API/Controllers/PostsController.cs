using CityDiscovery.SocialService.SocialServiceShared.Common.DTOs.Social;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialService.API.DTOs;
using SocialService.Application.Commands.CreatePost;
using SocialService.Application.Commands.DeletePost;
using SocialService.Application.Interfaces;
using SocialService.Application.Queries.GetAllPosts;
using SocialService.Application.Queries.GetPost;
using SocialService.Application.Queries.GetPostLikeCount;
using SocialService.Application.Queries.GetPostsByUser;
using SocialService.Application.Queries.GetPostsByVenue;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialService.API.Controllers
{
    /// <summary>
    /// Gönderiler API - Sosyal medya gönderilerini yönetir
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IImageService _imageService; // Servisi enjekte ediyoruz
        public PostsController(IMediator mediator, IImageService imageService)
        {
            _mediator = mediator;
            _imageService = imageService;
        }

        /// <summary>
        /// Yeni bir gönderi oluşturur
        /// </summary>
        /// <param name="command">Gönderi oluşturma komutu (UserId, VenueId, Content ve opsiyonel PhotoUrls içerir)</param>
        /// <returns>Oluşturulan gönderi ID'si</returns>
        /// <response code="201">Gönderi başarıyla oluşturuldu</response>
        /// <response code="400">Geçersiz istek veya mekan bulunamadı</response>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/Posts
        ///     {
        ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "venueId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "content": "Harika bir mekan! Kesinlikle tekrar geleceğim.",
        ///         "photoUrls": [
        ///             "https://example.com/photo1.jpg",
        ///             "https://example.com/photo2.jpg"
        ///         ]
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request)
        {

            try
            {
                // 1. Önce Token içerisinden kullanıcı ID'sini almayı dene
                // Not: Identity Service'te 'sub' veya 'NameIdentifier' olarak tutulur.
                // 1. Token içerisinden kullanıcı ID'sini al
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid finalUserId))
                {
                    return Unauthorized(new { error = "Kullanıcı kimliği doğrulanamadı." });
                }

                // 2. Fotoğrafları kaydetme işlemleri 
                var photoUrls = new List<string>();
                if (request.Photos != null && request.Photos.Count > 0)
                {
                    foreach (var file in request.Photos)
                    {
                        var savedPath = await _imageService.SaveImageAsync(file);
                        if (!string.IsNullOrEmpty(savedPath)) photoUrls.Add(savedPath);
                    }
                }

                // 3. Command oluştururken belirlediğimiz finalUserId'yi veriyoruz
                var command = new CreatePostCommand
                {
                    UserId = finalUserId,
                    VenueId = request.VenueId,
                    Content = request.Content,
                    PhotoUrls = photoUrls
                };

                var postId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetPostById), new { id = postId }, new { id = postId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        /// <summary>
        /// ID'ye göre bir gönderi getirir
        /// </summary>
        /// <param name="id">Gönderi ID'si</param>
        /// <returns>Gönderi detayları</returns>
        /// <response code="200">Gönderi bulundu</response>
        /// <response code="404">Gönderi bulunamadı</response>
        /// <remarks>
        /// Örnek yanıt:
        /// 
        ///     {
        ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "venueId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "authorUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "caption": "Harika bir mekan!",
        ///         "photoUrls": [
        ///             "https://example.com/photo1.jpg"
        ///         ],
        ///         "likeCount": 15,
        ///         "commentCount": 3,
        ///         "createdAt": "2024-01-15T10:30:00Z"
        ///     }
        /// </remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var query = new GetPostQuery { Id = id };
            var post = await _mediator.Send(query);

            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        /// <summary>
        /// Belirli bir mekana ait tüm gönderileri getirir
        /// </summary>
        /// <param name="venueId">Mekan ID'si</param>
        /// <returns>Mekana ait gönderi listesi</returns>
        /// <response code="200">Gönderiler başarıyla getirildi</response>
        /// <remarks>
        /// Örnek yanıt:
        /// 
        ///     [
        ///         {
        ///             "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "venueId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "authorUserId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "caption": "Harika bir mekan!",
        ///             "photoUrls": [],
        ///             "likeCount": 10,
        ///             "commentCount": 2,
        ///             "createdAt": "2024-01-15T10:30:00Z"
        ///         }
        ///     ]
        /// </remarks>
        [HttpGet("by-venue/{venueId}")]
        [ProducesResponseType(typeof(List<PostDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostsByVenue(Guid venueId)
        {
            var query = new GetPostsByVenueQuery { VenueId = venueId };
            var posts = await _mediator.Send(query);
            return Ok(posts);
        }

        /// <summary>
        /// Belirli bir gönderinin beğeni sayısını getirir
        /// </summary>
        /// <param name="postId">Gönderi ID'si</param>
        /// <returns>Gönderi ID'si ve beğeni sayısı</returns>
        /// <response code="200">Beğeni sayısı başarıyla getirildi</response>
        /// <remarks>
        /// Örnek yanıt:
        /// 
        ///     {
        ///         "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "likeCount": 25
        ///     }
        /// </remarks>
        [HttpGet("{postId}/likes/count")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostLikeCount(Guid postId)
        {
            var likeCount = await _mediator.Send(new GetPostLikeCountQuery { PostId = postId });
            return Ok(new { postId, likeCount });
        }


        /// <summary>
        /// Belirli bir kullanıcıya ait tüm gönderileri getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID'si</param>
        /// <returns>Kullanıcıya ait gönderi listesi</returns>
        /// <response code="200">Gönderiler başarıyla getirildi</response>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(List<PostDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostsByUser(Guid userId)
        {
            var query = new GetPostsByUserQuery { UserId = userId };
            var posts = await _mediator.Send(query);
            return Ok(posts);
        }


        /// <summary>
        /// Belirli bir gönderiyi siler
        /// </summary>
        /// <param name="id">Silinecek gönderinin ID'si</param>
        /// <response code="200">Gönderi başarıyla silindi</response>
        /// <response code="401">Yetkisiz işlem (Token geçersiz veya gönderi başkasına ait)</response>
        /// <response code="400">Geçersiz istek veya gönderi bulunamadı</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            try
            {
                // Token içerisinden kullanıcı ID'sini al
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new { error = "Kullanıcı kimliği doğrulanamadı." });
                }

                var command = new DeletePostCommand
                {
                    PostId = id,
                    UserId = userId
                };

                await _mediator.Send(command);

                return Ok(new { message = "Gönderi başarıyla silindi." });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Kullanıcı başkasının postunu silmeye çalışıyorsa
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Sistemdeki tüm gönderileri ana sayfa akışı (feed) için getirir
        /// </summary>
        /// <returns>Tüm gönderilerin listesi</returns>
        /// <response code="200">Gönderiler başarıyla getirildi</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<PostDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPosts()
        {
            var query = new GetAllPostsQuery();
            var posts = await _mediator.Send(query);
            return Ok(posts);
        }

        /// <summary>
        /// Bir gönderiyi kullanıcının kaydedilenlerine ekler
        /// </summary>
        [HttpPost("{postId}/save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SavePost(Guid postId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { error = "Kullanıcı kimliği doğrulanamadı." });

                var command = new CityDiscovery.SocialService.SocialService.Application.Commands.SavePost.SavePostCommand
                {
                    PostId = postId,
                    UserId = userId
                };

                await _mediator.Send(command);
                return Ok(new { message = "Gönderi kaydedildi.", isSaved = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Bir gönderiyi kullanıcının kaydedilenlerinden çıkarır
        /// </summary>
        [HttpDelete("{postId}/save")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UnsavePost(Guid postId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { error = "Kullanıcı kimliği doğrulanamadı." });

                var command = new SocialService.Application.Commands.UnsavePost.UnsavePostCommand
                {
                    PostId = postId,
                    UserId = userId
                };

                await _mediator.Send(command);
                return Ok(new { message = "Gönderi kaydedilenlerden çıkarıldı.", isSaved = false });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
    
