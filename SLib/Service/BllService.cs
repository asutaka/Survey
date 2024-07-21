using SLib.DAL;
using SLib.Model;
using SLib.Model.APIModel;
using SLib.Util;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
        Task<string> OnlyStock(Stock entity);
        Task<Stream> Chart_VonHoa_Category(string input);
        Task<Stream> Chart_LN_Category(string input);

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
        private readonly IConfigDataRepo _configRepo;
        private readonly ITuDoanhRepo _tudoanhRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IFinancialRepo _financialRepo;
        private readonly IFileService _fileService;
        public BllService(IAPIService apiService,
                            IStockRepo stockRepo,
                            IConfigDataRepo configRepo,
                            ITuDoanhRepo tudoanhRepo,
                            IFinancialRepo financialRepo,
                            ICategoryRepo categoryRepo,
                            IFileService fileService
                            )
        {
            _apiService = apiService;
            _stockRepo = stockRepo;
            _configRepo = configRepo;
            _tudoanhRepo = tudoanhRepo;
            _categoryRepo = categoryRepo;
            _financialRepo = financialRepo;
            _fileService = fileService;
        }

        public List<Stock> GetStock()
        {
            return _stockRepo.GetAll();
        }

        public async Task Test()
        {
            //var lLoiNhuan = await _apiService.ThongKeLoiNhuan("DPG");
            //lLoiNhuan.Reverse();
            //foreach (var item in lLoiNhuan)
            //{

            //}
            //var tmp1 = 1;
        }
    }
}
