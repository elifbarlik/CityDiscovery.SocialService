using CityDiscovery.SocialService.SocialService.Infrastructure.Messaging;
using SocialService.Application.Interfaces;
using SocialService.Infrastructure.HttpClients;
using SocialService.Infrastructure.Messaging;
using SocialService.Infrastructure.Repositories;
using SocialService.Infrastructure.Services;


namespace SocialService.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // IPostRepository istendiğinde, ona PostRepository'nin bir örneğini ver.
            // Scoped: Her bir HTTP isteği için bir tane örnek oluşturur.
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<IImageService, LocalImageService>();
            services.AddHttpClient<IVenueServiceClient, VenueServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5275");
            });

            // HttpClient'ı ve IdentityServiceClient'ı kaydediyoruz.
            services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>(client =>
            {
                // IdentityService'in adresini appsettings.json'dan alacağız.
                // Şimdilik buraya bir örnek adres yazalım.
                client.BaseAddress = new Uri("http://localhost:5000");
            });

            // MessageBus'ı kaydediyoruz.
            services.AddScoped<IMessageBus, MessageBus>();

            return services;
        }
    }
}