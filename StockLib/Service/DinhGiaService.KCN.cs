using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private (EPoint, string) DG_KCN(string code)
        {
            try
            {
                var special = _specRepo.GetEntityByFilter(Builders<SpecialInfo>.Filter.Eq(x => x.s, code));
                if(special is null)
                {
                    return (EPoint.Unknown, string.Empty);
                }

                var lFDI = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.FDI));
                if (lFDI is null || !lFDI.Any())
                {
                    return (EPoint.Unknown, string.Empty);
                }

                var lEpoint = new List<EPoint>();
                var dMax = lFDI.MaxBy(x => x.d).d;
                foreach (var item in special.locate)
                {
                    var cur = lFDI.FirstOrDefault(x => x.d == dMax && x.content.RemoveSignVietnamese().RemoveSpace().Contains(item.RemoveSignVietnamese().RemoveSpace(), StringComparison.OrdinalIgnoreCase));
                    if (cur is null)
                        continue;

                    var prev = lFDI.FirstOrDefault(x => x.d == dMax - 100 && x.content == cur.content);
                    if(prev is null)
                    {
                        lEpoint.Add(EPoint.VeryPositive);
                        continue;
                    }

                    var rate = Math.Round(100 * (-1 + cur.va / prev.va), 1);
                    if(rate >= 15)
                    {
                        lEpoint.Add(EPoint.VeryPositive);
                    }
                    else if(rate >= 5)
                    {
                        lEpoint.Add(EPoint.Positive);
                    }
                    else if (rate > -5)
                    {
                        lEpoint.Add(EPoint.Normal);
                    }
                    else if (rate > -15)
                    {
                        lEpoint.Add(EPoint.Negative);
                    }
                    else
                    {
                        lEpoint.Add(EPoint.VeryNegative);
                    }
                }
                if(!lEpoint.Any())
                {
                    return (EPoint.Unknown, string.Empty);
                }

                var lTop5 = lFDI.OrderByDescending(x => x.d).ThenByDescending(x => x.va).Take(5);
                var sBuilder = new StringBuilder();
                foreach (var item in lTop5)
                {
                    sBuilder.AppendLine($"   - {item.content}({item.va})");
                }

                var avg = lEpoint.Average(x => (int)x);
                if(avg > (int)EPoint.Positive)
                {
                    return (EPoint.VeryPositive, sBuilder.ToString());
                }
                if (avg > (int)EPoint.Normal)
                {
                    return (EPoint.Positive, sBuilder.ToString());
                }
                if (avg > (int)EPoint.Negative)
                {
                    return (EPoint.Normal, sBuilder.ToString());
                }
                if (avg > (int)EPoint.VeryNegative)
                {
                    return (EPoint.Negative, sBuilder.ToString());
                }
                return (EPoint.VeryNegative, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_KCN|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
