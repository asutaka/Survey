using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StockLib.DAL.Entity;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class DinhGiaService
    {
        private (EPoint, string) DG_BDS(string code)
        {
            try
            {
                var step1 = 5;
                var step2 = 15;
                //Người mua trả tiền trước
                var lBDS = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
                if (lBDS is null || lBDS.Count() < 5)
                {
                    return (EPoint.VeryNegative, string.Empty);
                }
                lBDS = lBDS.OrderByDescending(x => x.d).Take(5).ToList();
                var first = lBDS.First().bp;
                var second = lBDS.Skip(1).First().bp;
                var third = lBDS.Skip(2).First().bp;
                var four = lBDS.Skip(3).First().bp;
                var five = lBDS.Last().bp;

                var countGreat = 0;
                var countLess = 0;
                var isGreate = false;
                if (first > second * 1.2 && first >= 200)
                    isGreate = true;

                if (first > second * 1.05 && first >= 500)
                    countGreat++;
                if (second > third * 1.05 && second >= 100)
                    countGreat++;
                if (third > four * 1.05 && third >= 100)
                    countGreat++;
                if (four > five * 1.05 && four >= 100)
                    countGreat++;

                if (first * 1.05 < second && second >= 100)
                    countLess++;
                if (second * 1.05 < third && third >= 100)
                    countLess++;
                if (third * 1.05 < four && four >= 100)
                    countLess++;
                if (four * 1.05 < five && five >= 100)
                    countLess++;

                if (countGreat >= 3) 
                {
                    return (EPoint.VeryPositive, $"   - Tăng trưởng người mua({countGreat}/5)");
                }
                else if (isGreate) 
                {
                    return (EPoint.Positive, $"   - Tăng trưởng người mua đột biến quý gần nhất");
                }
                else if(countLess >= 4)
                {
                    return (EPoint.VeryNegative, $"   - Suy giảm người mua({countLess}/5)");
                }
                else if (countLess == 3)
                {
                    return (EPoint.Negative, $"   - Suy giảm người mua({countLess}/5)");
                }
                else
                {
                    return (EPoint.Normal, $"   - Chưa có đột biến người mua");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DinhGiaService.DG_BDS|EXCEPTION| {ex.Message}");
            }
            return (EPoint.Unknown, string.Empty);
        }
    }
}
