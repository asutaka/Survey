using Microsoft.Extensions.DependencyInjection;

namespace SurveyStock.Service
{
    public static class ServiceDI
    {
        public static void ServiceDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IDataAPIService, DataAPIService>();
            services.AddSingleton<IBllService, BllService>();
            services.AddSingleton<IFileService, FileService>();
        }
    }
}
