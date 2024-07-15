using Microsoft.Extensions.DependencyInjection;

namespace SLib.Service
{
    public static class ServiceDI
    {
        public static void ServiceDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAPIService, APIService>();
            services.AddSingleton<IBllService, BllService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ITelegramLibService, TelegramLibService>();
            services.AddSingleton<IBridgeService, BridgeService>();
        }
    }
}
