using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        //Tổng mức bán lẻ + Tồn kho + Biên lợi nhuận
        private (EPoint, string) DG_BanLe(string code)
        {
            try
            {
                var step1 = 5;
                var step2 = 15;
                //Tổng mức
                var tongmuc =  ModeThongKe(EKeyTongCucThongKe.BanLe, step1, step2);
                //Tồn kho
                var lBanLe = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
                if(lBanLe is null || !lBanLe.Any())
                {
                    return (EPoint.VeryNegative, string.Empty);
                }
                var cur = lBanLe.MaxBy(x => x.d);
                var prev = lBanLe.FirstOrDefault(x => x.d == cur.d - 10);
                if (prev is null || prev.inv <= 0 || prev.rv <= 0)
                {
                    return (EPoint.VeryNegative, string.Empty);
                }

                var rateTonKho = Math.Round(100 * (-1 + cur.inv / prev.inv), 1);
                var tonkho = EPointResponse(rateTonKho, step1, step2, "Tồn kho cùng kỳ");
                //Biên lợi nhuận
                var curBienLoiNhuan = Math.Round(100 * cur.pfg / cur.rv, 1);
                var prevBienLoiNhuan = Math.Round(100 * prev.pfg / prev.rv, 1);
                var rateBienLoiNhuan = Math.Round(100 * (-1 + curBienLoiNhuan / prevBienLoiNhuan));
                var bienloinhuan = EPointResponse(rateBienLoiNhuan, step1, step2, "Biên lợi nhuận gộp cùng kỳ");

                var sBuilder = new StringBuilder();
                sBuilder.AppendLine(tongmuc.Item2);
                sBuilder.AppendLine(tonkho.Item2);
                sBuilder.AppendLine(bienloinhuan.Item2);

                var merge = MergeEnpoint(tongmuc.Item1, tonkho.Item1, bienloinhuan.Item1);
                return (merge, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_BanLe|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
