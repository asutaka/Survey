using Microsoft.Extensions.Logging;
using SLib.DAL;
using SLib.Model;
using SLib.Model.APIModel;
using SLib.Util;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SLib.Service
{
    public interface IBllService
    {
        Task<(int, string)> SyncTuDoanhHNX();
        Task<(int, string)> SyncTuDoanhUp();
        Task<(int, List<string>)> SyncTuDoanhHSX();
        Task<(int, string)> SyncThongkeGDNN(E24hGDNNType type);
        Task<(int, string)> SyncThongkeNhomNganh(E24hGDNNType type);
        Task<(int, string)> LayMaTheoChiBao();
        Task<(int, List<string>)> ChiBaoKyThuat();

        Task DongBoNgayCongBoBCTC();
        Task DongBoDoanhThuLoiNhuan();

        //Tele only stock
        List<Stock> GetStock();
        Task BCTCRead(string path);
        Task OnlyStock(long userID, Stock entity, TelegramBotClient bot);
        Task<Stream> Chart_VonHoa_Category(string input);
        Task<Stream> Chart_LN_Category(string input);
        Task<Stream> Chart_GG_Category(string input, string name, string sheetName);
        Task<Stream> Chart_ChienLuocDauTu();

        Task<Stream> Chart_LN_Stock(string code);
        Task<Stream> Chart_GG_Stock(string code, string name, string sheetName, bool isShowIncrease = false);
        Task<Stream> Chart_KeHoachNam_Stock(string code, List<KeHoachThucHienAPIData> lData);

        //HighChart
        Task<Stream> GetBasicColumn();
        Task<Stream> GetBasicColumnCustomColor();


        string TongTuDoanhStr();
        string TongGDNNStr();
        string TuDoanhBuildStr(string code);
        string ForeignBuildStr(string code);
        string ThongKeThiTruongStr();
        Task Test();
    }
    public partial class BllService : IBllService
    {
        private readonly IAPIService _apiService;
        private readonly IStockRepo _stockRepo;
        private readonly IConfigBCTCRepo _configBCTCRepo;
        private readonly IConfigDataRepo _configRepo;
        private readonly ITuDoanhRepo _tudoanhRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IFinancialRepo _financialRepo;
        private readonly IFileService _fileService;
        private readonly IGoogleService _googleService;
        private readonly IGoogleDataRepo _ggDataRepo;
        private readonly IStockRevenueRepo _stockRevenueRepo;
        private readonly ILogger<BllService> _logger;
        public BllService(ILogger<BllService> logger, 
                            IAPIService apiService,
                            IStockRepo stockRepo,
                            IConfigBCTCRepo configBCTCRepo,
                            IConfigDataRepo configRepo,
                            ITuDoanhRepo tudoanhRepo,
                            IStockRevenueRepo stockRevenueRepo,
                            IFinancialRepo financialRepo,
                            ICategoryRepo categoryRepo,
                            IFileService fileService,
                            IGoogleDataRepo ggDataRepo,
                            IGoogleService googleService
                            )
        {
            _logger = logger;
            _apiService = apiService;
            _stockRepo = stockRepo;
            _configRepo = configRepo;
            _configBCTCRepo = configBCTCRepo;
            _tudoanhRepo = tudoanhRepo;
            _categoryRepo = categoryRepo;
            _financialRepo = financialRepo;
            _fileService = fileService;
            _googleService = googleService;
            _ggDataRepo = ggDataRepo;
            _stockRevenueRepo = stockRevenueRepo;
        }

        public List<Stock> GetStock()
        {
            return _stockRepo.GetAll();
        }

        public async Task Test()
        {
            //_googleService.GGDoanhThu("NganHang");
            //_googleService.GGLoiNhuan("NganHang");
            //_googleService.GGDoanhThu("ChungKhoan");
            //_googleService.GGLoiNhuan("ChungKhoan");
            //_googleService.GGDoanhThu("BatDongSan");
            //_googleService.GGLoiNhuan("BatDongSan");


            //_googleService.Post(new Model.GoogleSheet.Item { Id = "156", Category = "abc", Name = "zzz", Price = "-3" });

            //_googleService.Put(2, new Model.GoogleSheet.Item { Id = "1", Category = "abc", Name = "zzz", Price = "9" });
            //var lLoiNhuan = await _apiService.ThongKeLoiNhuan("DPG");
            //lLoiNhuan.Reverse();
            //foreach (var item in lLoiNhuan)
            //{

            //}
            //var tmp1 = 1;
        }
    }
}
