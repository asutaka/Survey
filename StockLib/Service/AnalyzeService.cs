﻿using Microsoft.Extensions.Logging;
using StockLib.DAL;

namespace StockLib.Service
{
    public interface IAnalyzeService
    {
        Task<(int, string)> ChiBaoMA20();
        Task<(int, string)> ChiBao52W();
        Task<(int, string)> ThongkeNhomNganh(DateTime dt);
        Task<(int, string)> ThongkeForeign(DateTime dt);
        Task<(int, string)> ThongKeTuDoanhHNX(DateTime dt);
        Task<(int, string)> ThongKeTuDoanhUp(DateTime dt);
        Task<(int, string)> ThongKeTuDoanhHSX(DateTime dt);
        Task<(int, string)> TongCucThongKe(DateTime dt);
    }
    public partial class AnalyzeService : IAnalyzeService
    {
        private readonly ILogger<AnalyzeService> _logger;
        private readonly IAPIService _apiService;
        private readonly IFileService _fileService;
        private readonly IStockRepo _stockRepo;
        private readonly IConfigDataRepo _configRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly ITuDoanhRepo _tudoanhRepo;
        public AnalyzeService(ILogger<AnalyzeService> logger,
                            IAPIService apiService,
                            IFileService fileService,
                            IStockRepo stockRepo,
                            IConfigDataRepo configRepo,
                            ICategoryRepo categoryRepo,
                            ITuDoanhRepo tudoanhRepo) 
        {
            _logger = logger;
            _apiService = apiService;
            _fileService = fileService;
            _stockRepo = stockRepo;
            _configRepo = configRepo;
            _categoryRepo = categoryRepo;
            _tudoanhRepo = tudoanhRepo;
        }
    }
}
