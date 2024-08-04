using Microsoft.Extensions.DependencyInjection;

namespace StockLib.Service.Settings
{
    public static class RegisterService
    {
        public static void ServiceDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAPIService, APIService>();
            services.AddSingleton<IBllService, BllService>();
        }
    }
}
