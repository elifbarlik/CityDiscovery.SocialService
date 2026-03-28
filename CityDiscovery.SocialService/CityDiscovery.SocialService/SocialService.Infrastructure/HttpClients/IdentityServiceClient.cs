//using CityDiscovery.SocialService.SocialService.Application.DTOs;
//using SocialService.Application.Interfaces;
//using System;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;

//namespace SocialService.Infrastructure.HttpClients
//{
//    public class IdentityServiceClient : IIdentityServiceClient
//    {
//        private readonly HttpClient _httpClient;

//        public IdentityServiceClient(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<UserDto> GetUserAsync(Guid userId)
//        {
//            try
//            {
//                // Gerçek senaryoda, IdentityService'in ilgili endpoint'ini çağırırız.
//                // Örneğin: var response = await _httpClient.GetAsync($"/api/users/{userId}");
//                // return await response.Content.ReadFromJsonAsync<UserDto>();

//                // ŞİMDİLİK, IdentityService hazır olmadığı için, test amacıyla null döndürelim.
//                return await Task.FromResult<UserDto>(null);
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public async Task<bool> CheckUserExistsAsync(Guid userId)
//        {
//            try
//            {
//                // Gerçek senaryoda, IdentityService'in ilgili endpoint'ini çağırırız.
//                // Örneğin: var response = await _httpClient.GetAsync($"/api/users/{userId}/exists");
//                // return response.IsSuccessStatusCode;

//                // ŞİMDİLİK, IdentityService hazır olmadığı için, test amacıyla her zaman doğru varsayalım.
//                return await Task.FromResult(true);
//            }
//            catch
//            {
//                return false;
//            }
//        }
//    }
//}


//using CityDiscovery.SocialService.SocialService.Application.DTOs;
//using SocialService.Application.Interfaces;
//using System;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;

//namespace SocialService.Infrastructure.HttpClients
//{
//    public class IdentityServiceClient : IIdentityServiceClient
//    {
//        private readonly HttpClient _httpClient;

//        public IdentityServiceClient(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<UserDto> GetUserAsync(Guid userId)
//        {
//            try
//            {
//                // Test amacıyla null dönen kısım kaldırıldı, gerçek API çağrısı aktif edildi.
//                var response = await _httpClient.GetAsync($"/api/users/{userId}");

//                if (response.IsSuccessStatusCode)
//                {
//                    return await response.Content.ReadFromJsonAsync<UserDto>();
//                }

//                return null;
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public async Task<bool> CheckUserExistsAsync(Guid userId)
//        {
//            try
//            {
//                // Test amacıyla her zaman true dönen kısım kaldırıldı, gerçek API çağrısı aktif edildi.
//                // Not: Eğer IdentityService tarafında "exists" adında özel bir endpoint'iniz yoksa
//                // burayı da "/api/users/{userId}" olarak bırakabilirsiniz.
//                var response = await _httpClient.GetAsync($"/api/users/{userId}/exists");

//                return response.IsSuccessStatusCode;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//    }
//}
using CityDiscovery.SocialService.SocialService.Application.DTOs;
using SocialService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SocialService.Infrastructure.HttpClients
{
    public class IdentityServiceClient : IIdentityServiceClient
    {
        private readonly HttpClient _httpClient;

        public IdentityServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDto> GetUserAsync(Guid userId)
        {
            try
            {
                // [Authorize] takılmasına karşı [AllowAnonymous] olan 'bulk' endpoint'ini kullanıyoruz.
                // Tek bir ID gönderip, dönen listeden ilk kullanıcıyı alıyoruz.
                var response = await _httpClient.PostAsJsonAsync("/api/users/bulk", new List<Guid> { userId });

                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    return users?.FirstOrDefault(); // Listeden ilk kullanıcıyı dön
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[IdentityService HATA] Status: {response.StatusCode}, Detay: {errorContent}");
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CheckUserExistsAsync(Guid userId)
        {
            try
            {
                // Sizin kodunuzdaki exists endpoint'i zaten [AllowAnonymous] olduğu için sorunsuz çalışacaktır.
                var response = await _httpClient.GetAsync($"/api/users/{userId}/exists");

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}