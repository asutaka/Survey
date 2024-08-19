using Microsoft.Extensions.Logging;

namespace StockLib.Service
{
    public interface IPartternService
    {

    }
    public partial class PartternService : IPartternService
    {
        private readonly ILogger<PartternService> _logger;
        public PartternService(ILogger<PartternService> logger) 
        {
            _logger = logger;
        }
    }
}
