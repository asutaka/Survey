using SLib.DAL;
using SLib.Model;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Task<(int, string)> KeHoachNam(string code);
        Task<(int, string)> ThongKeLoiNhuan(string code);
        Task<(int, List<string>)> ChiBaoKyThuat();

        //Tele only stock
        List<Stock> GetStock();

        //public
        Task<(int, string)> TA(string code);
        Task<(int, string)> ThongKeGD(string code);
        Task<(int, string)> FA(string code);
        Task<(int, string)> ThongKeKhac(string code);
        Task<(int, string)> PTChuyenSau(string code);






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
        //private readonly IReportMongoRepo _reportRepo;
        private readonly IFileService _fileService;
        public BllService(IAPIService apiService,
                            IStockRepo stockRepo,
                            IConfigDataRepo configRepo,
                            ITuDoanhRepo tudoanhRepo,
                            //IReportMongoRepo reportRepo,
                            ICategoryRepo categoryRepo,
                            IFileService fileService
                            )
        {
            _apiService = apiService;
            _stockRepo = stockRepo;
            _configRepo = configRepo;
            _tudoanhRepo = tudoanhRepo;
            _categoryRepo = categoryRepo;
            //_reportRepo = reportRepo;
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
