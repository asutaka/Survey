using NLog.Web;
using Quartz;
using StockBridgeAPI.Jobs;
using StockBridgeAPI.Utils;

namespace StockBridgeAPI
{
    public class Program
    {
        [Obsolete]
        public static void Main(string[] args)
        {
            //var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                Console.WriteLine("init main function");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error in init");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        [Obsolete]
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {})
                    .UseStartup<Startup>()
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Information);
                    })
                    .UseNLog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Add the required Quartz.NET services
                    services.AddQuartz(q =>
                    {
                        // Use a Scoped container to create jobs. I'll touch on this later
                        q.UseMicrosoftDependencyInjectionScopedJobFactory();
                        q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 2; });

                        // Register the job, loading the schedule from configuration
                        q.AddJobAndTrigger<TelegramJob>(hostContext.Configuration);
                        //q.AddJobAndTrigger<BackgroundJob>(hostContext.Configuration);
                    });

                    // Add the Quartz.NET hosted service

                    services.AddQuartzHostedService(
                        q => q.WaitForJobsToComplete = true);
                    // other config


                });
    }
}

