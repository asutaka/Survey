﻿using Microsoft.Extensions.Logging;
using StockLib.DAL;
using StockLib.Utils;

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
        Task<(int, string)> TongCucThongKeThang(DateTime dt);
        Task TongCucThongKeThangHis();
        Task<(int, string)> TongCucThongKeQuy(DateTime dt);
        Task TongCucThongKeQuyHis();
        Task<(int, string)> TongCucHaiQuan(DateTime dt, EConfigDataType mode);
        Task<(int, string)> TinHieuMuaBan();
        Task<(int, string)> BaoCaoPhanTich(DateTime dt);
        Task<(int, string)> TraceGia(DateTime dt);

        Task<(int, string)> TinHieuMuaBanCoin();
    }
    public partial class AnalyzeService : IAnalyzeService
    {
        private readonly ILogger<AnalyzeService> _logger;
        private readonly IAPIService _apiService;
        private readonly IFileService _fileService;
        private readonly IStockRepo _stockRepo;
        private readonly IConfigDataRepo _configRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IThongKeRepo _thongkeRepo;
        private readonly IThongKeQuyRepo _thongkeQuyRepo;
        private readonly IThongKeHaiQuanRepo _haiquanRepo;
        private readonly IConfigBaoCaoPhanTichRepo _bcptRepo;
        private readonly ICoinRepo _coinRepo;
        public AnalyzeService(ILogger<AnalyzeService> logger,
                            IAPIService apiService,
                            IFileService fileService,
                            IStockRepo stockRepo,
                            ICoinRepo coinRepo,
                            IConfigDataRepo configRepo,
                            ICategoryRepo categoryRepo,
                            IThongKeRepo thongkeRepo,
                            IThongKeHaiQuanRepo haiquanRepo,
                            IThongKeQuyRepo thongkeQuyRepo,
                            IConfigBaoCaoPhanTichRepo bcptRepo) 
        {
            _logger = logger;
            _apiService = apiService;
            _fileService = fileService;
            _stockRepo = stockRepo;
            _coinRepo = coinRepo;
            _configRepo = configRepo;
            _categoryRepo = categoryRepo;
            _thongkeRepo = thongkeRepo;
            _thongkeQuyRepo = thongkeQuyRepo;
            _haiquanRepo = haiquanRepo;
            _bcptRepo = bcptRepo;
        }
    }
}
