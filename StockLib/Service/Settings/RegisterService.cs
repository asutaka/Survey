using Microsoft.Extensions.DependencyInjection;

namespace StockLib.Service.Settings
{
    public static class RegisterService
    {
        public static void ServiceDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAPIService, APIService>();
            services.AddSingleton<IBllService, BllService>();
            services.AddSingleton<ITeleService, TeleService>();
            services.AddSingleton<ICalculateService, CalculateService>();
            services.AddSingleton<IAnalyzeService, AnalyzeService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IPartternService, PartternService>();
            services.AddSingleton<IDinhGiaService, DinhGiaService>();
        }
    }
}
