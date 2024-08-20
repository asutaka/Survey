using Microsoft.Extensions.Logging;

namespace StockLib.Service
{
    public interface IPartternService
    {
        Task SurveyIchimoku(string code);
        Task SurveySuperTrend(string code);
    }
    public partial class PartternService : IPartternService
    {
        private readonly ILogger<PartternService> _logger;
        private readonly IAPIService _apiService;
        public PartternService(ILogger<PartternService> logger,
                                IAPIService apiService) 
        {
            _logger = logger;
            _apiService = apiService;
        }
    }
}
