using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockLibrary.DAL;
using StockLibrary.Service;

namespace BridgeStock
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;

            Application.Run(ServiceProvider.GetRequiredService<frmMain>());
        }
        public static IServiceProvider ServiceProvider { get; private set; }
        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    services.DALDependencies();
                    services.ServiceDependencies();
                    services.AddSingleton<frmMain>();
                });
        }
    }
}