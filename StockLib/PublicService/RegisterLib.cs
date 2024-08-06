using Microsoft.Extensions.DependencyInjection;
using StockLib.DAL;
using StockLib.DAL.Settings;
using StockLib.PublicService.Settings;
using StockLib.Service.Settings;

namespace StockLib.PublicService
{
    public static class RegisterLib
    {
        public static void AddSLib(this IServiceCollection services)
        {
            services.DALDependencies();
            services.ServiceDependencies();
            services.PublicServiceDependencies();
        }
    }
}
