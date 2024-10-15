using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<List<Stream>> Chart_MaCK(string input)
        {
            try
            {
                var stock = StaticVal._lStock.FirstOrDefault(x => x.s == input);
                if (stock == null)
                    return null;

                var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, input));
                if (!lFinancial.Any())
                    return null;

                var lStream = new List<Stream>();
                var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf }).ToList(), input);
                lStream.Add(streamDoanhThu);

                var streamNoTaiChinh = await Chart_NoTaiChinh(lFinancial, input);
                lStream.Add(streamNoTaiChinh);

                if (stock.IsTonKho())
                {
                    var stream = await Chart_TonKho(lFinancial, input);
                    lStream.Add(stream);
                }

                if (stock.IsFDI())
                {
                    var stream = await Chart_FDI();
                    lStream.Add(stream);
                }

                if (stock.IsNguoiMua())
                {
                    var stream = await Chart_NguoiMua(lFinancial, input);
                    lStream.Add(stream);
                }

                if (stock.IsXNK())
                {
                    var stream = await Chart_XNK(stock);
                    lStream.Add(stream);
                }

                if(stock.IsBanLe())
                {
                    var stream = await Chart_ThongKe_BanLe();
                    lStream.Add(stream);
                }

                if (stock.IsCangBien())
                {
                    var stream = await Chart_ThongKeQuy_CangBien();
                    lStream.Add(stream);
                }

                if (stock.IsHangKhong())
                {
                    var stream = await Chart_ThongKeQuy_HangKhong();
                    lStream.Add(stream);
                }

                if (stock.IsCaoSu())
                {
                    var stream = await Chart_ThongKeQuy_HangKhong();
                    lStream.Add(stream);
                }

                if (stock.IsNganHang())
                {
                    lStream.AddRange(await Chart_NganHang(input));
                }

                if (stock.IsChungKhoan())
                {
                    lStream.AddRange(await Chart_ChungKhoan(input));
                }

                return lStream;
            }
            catch(Exception ex)
            {
                _logger.LogError($"BllService.Chart_MaCK|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
