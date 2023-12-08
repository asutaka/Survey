using Microsoft.Extensions.Configuration;

namespace Survey.Utils
{
    public class Helper
    {
        private static IConfiguration _Configuration;
        public static IConfiguration GetConfig()
        {
            if (_Configuration != null)
                return _Configuration;
            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _Configuration = builder.Build();
            return _Configuration;
        }

    }
}
