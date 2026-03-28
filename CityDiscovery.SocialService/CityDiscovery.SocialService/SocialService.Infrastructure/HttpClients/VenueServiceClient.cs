using CityDiscovery.SocialService.SocialService.Application.DTOs; // DTO'nun olduğu yer
using SocialService.Application.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SocialService.Infrastructure.HttpClients
{
    public class VenueServiceClient : IVenueServiceClient
    {
        private readonly HttpClient _httpClient;

        public VenueServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. Mekan Detaylarını Getir (Gerçek Bağlantı)
        public async Task<VenueDto> GetVenueAsync(Guid venueId)
        {
            try
            {
                // Venue Service'e GET isteği atıyoruz
                var response = await _httpClient.GetAsync($"api/venues/{venueId}");

                if (!response.IsSuccessStatusCode)
                {
                    // Mekan bulunamazsa null dön
                    return null;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[VenueService HATA] Status: {response.StatusCode}, Detay: {errorContent}");
                // Gelen veriyi DTO'ya çevir
                return await response.Content.ReadFromJsonAsync<VenueDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VenueService request failed: {ex.Message}");
                return null;
            }
        }

        // 2. Mekan Var mı?
        public async Task<bool> VenueExistsAsync(Guid venueId)
        {
            var venue = await GetVenueAsync(venueId);
            return venue != null;
        }

        // 3. Interface Uyumluluğu
        public async Task<bool> CheckVenueExistsAsync(Guid venueId)
        {
            return await VenueExistsAsync(venueId);
        }

        // 4. Mekan Sahibini Getir
        public async Task<Guid> GetVenueOwnerAsync(Guid venueId)
        {
            var venue = await GetVenueAsync(venueId);
            if (venue == null) return Guid.Empty;

            return venue.OwnerId;
        }
    }
}