using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private async Task<(EPoint, string)> DG_PhanBon(string code)
        {
            try
            {
                var xk = XNK(EStockType.PhanBon, 5, 15);

                var spec = _specRepo.GetEntityByFilter(Builders<SpecialInfo>.Filter.Eq(x => x.s, code));
                if(spec is null)
                {
                    return (EPoint.Unknown, string.Empty);
                }

                (EPoint, string) ure = (EPoint.Unknown, string.Empty);
                if (spec.products.Any(x => x.RemoveSignVietnamese().RemoveSpace().Contains("ure", StringComparison.OrdinalIgnoreCase)))
                {
                    var ure_qoq = await _apiService.Tradingeconimic_GetForex("urea");
                    ure = EPointResponse(ure_qoq, 5, 15, "Giá Ure");
                }
                (EPoint, string) oil = (EPoint.Unknown, string.Empty);
                if (spec.materials.Any(x => x.RemoveSignVietnamese().RemoveSpace().Contains("khi", StringComparison.OrdinalIgnoreCase)))
                {
                    oil = await DinhGia_Forex(EForex.CL, 5, 15);
                }
                (EPoint, string) than = (EPoint.Unknown, string.Empty);
                if (spec.materials.Any(x => x.RemoveSignVietnamese().RemoveSpace().Contains("than", StringComparison.OrdinalIgnoreCase)))
                {
                    var than_qoq = await _apiService.Tradingeconimic_GetForex("coal");
                    than = EPointResponse(than_qoq, 5, 15, "Giá Than");
                }

                var sBuilder = new StringBuilder();
                (EPoint, string) total = (EPoint.Unknown, string.Empty);
                if (oil.Item1 != EPoint.Unknown)
                {
                    total = oil;
                    sBuilder.AppendLine(oil.Item2);
                }
                if (than.Item1 != EPoint.Unknown) 
                {
                    sBuilder.AppendLine(than.Item2);
                    if (total.Item1 == EPoint.Unknown)
                    {
                        total = than;
                    }
                    else
                    {
                        total = (MergeEnpoint(total.Item1, than.Item1), sBuilder.ToString());
                    }
                }
                if (ure.Item1 != EPoint.Unknown) 
                {
                    sBuilder.AppendLine(ure.Item2);
                    if (total.Item1 == EPoint.Unknown)
                    {
                        total = ure;
                    }
                    else
                    {
                        total = (MergeEnpoint(total.Item1, ure.Item1), sBuilder.ToString());
                    }
                }

                return (MergeEnpoint(xk.Item1, total.Item1), sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_PhanBon|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
