using SLib.DAL;
using SLib.Util;
using System.Collections.Generic;
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

        string TongTuDoanhStr();
        string TongGDNNStr();
        string TuDoanhBuildStr(string code);
        string ForeignBuildStr(string code);
        string ThongKeThiTruongStr();
    }
    public partial class BllService : IBllService
    {
        private readonly IAPIService _apiService;
        //private readonly IStockRepo _stockRepo;
        private readonly IConfigDataRepo _configRepo;
        private readonly ITuDoanhRepo _tudoanhRepo;
        private readonly ICategoryRepo _categoryRepo;
        //private readonly IReportMongoRepo _reportRepo;
        private readonly IFileService _fileService;
        public BllService(IAPIService apiService,
                            //IStockRepo stockRepo,
                            IConfigDataRepo configRepo,
                            ITuDoanhRepo tudoanhRepo,
                            //IReportMongoRepo reportRepo,
                            ICategoryRepo categoryRepo,
                            IFileService fileService
                            )
        {
            _apiService = apiService;
            //_stockRepo = stockRepo;
            _configRepo = configRepo;
            _tudoanhRepo = tudoanhRepo;
            _categoryRepo = categoryRepo;
            //_reportRepo = reportRepo;
            _fileService = fileService;
        }
    }
}
