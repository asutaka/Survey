using MongoDB.Driver;
using Skender.Stock.Indicators;
using SLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<(int, string)> ChiBaoKyThuat()
        {
            try
            {
                var strOutput = new StringBuilder();
                var lReport = new List<ReportPTKT>();
                var dtStart = DateTime.Now;
                FilterDefinition<Stock> filter = Builders<Stock>.Filter.Gte(x => x.status, 0);
                var lStock = _stockRepo.GetByFilter(filter).OrderBy(x => x.rank);
                int dem = 0;
                foreach (var item in lStock)
                {
                    try
                    {
                        
                        var lData = await _apiService.GetDataStock(item.s);
                        if (lData.Count() < 250
                            || lData.Last().Volume < 50000)
                            continue;
                        var model = new ReportPTKT
                        {
                            s = item.s,
                            rank = item.rank
                        };

                        var lIchi = lData.GetIchimoku();
                        var lBb = lData.GetBollingerBands();
                        var lEma21 = lData.GetEma(21);
                        var lEma50 = lData.GetEma(50);
                        var lEma200 = lData.GetEma(200);
                        var lRsi = lData.GetRsi();

                        //Chỉ báo bắt đáy
                        var lBBCheck = lBb.TakeLast(7);
                        var lEma50Check = lEma50.TakeLast(7);
                        for (int i = 0; i < lBBCheck.Count(); i++)
                        {
                            var itemBBCheck = lBBCheck.ElementAt(i);
                            var itemEma50Check = lEma50Check.ElementAt(i);
                            if ((itemBBCheck.LowerBand ?? 0) > (itemEma50Check.Ema ?? 0))
                            {
                                model.bd = true;
                                break;
                            }
                        }
                        //MA20
                        var entity = lData.Last();
                        var bb = lBb.Last();
                        model.isGEMA20 = entity.Close >= (decimal)bb.Sma;
                        model.isCrossMa20Up = entity.Close >= (decimal)bb.Sma && entity.Open <= (decimal)bb.Sma;
                        model.isPriceUp = entity.Close > entity.Open;
                        if (entity.Close < (decimal)bb.LowerBand)
                        {
                            model.priceBB = 1;
                        }
                        else if (entity.Close > (decimal)bb.UpperBand)
                        {
                            model.priceBB = 3;
                        }
                        else
                        {
                            model.priceBB = 2;
                        }
                        //RSI zero
                        var lRsiCheck = lRsi.TakeLast(7);
                        for (int i = 0; i < lRsiCheck.Count() - 1; i++)
                        {
                            if ((lRsiCheck.ElementAt(i).Rsi - lRsiCheck.ElementAt(i + 1).Rsi) == 0)
                            {
                                model.isRsiZero = true;
                                break;
                            }
                        }
                        //Ichi
                        var ichiCheck = lIchi.Last();
                        if (entity.Close < ichiCheck.SenkouSpanA && entity.Close < ichiCheck.SenkouSpanB)
                        {
                            model.priceCompareIchi = 1;
                        }
                        else if (entity.Close > ichiCheck.SenkouSpanA && entity.Close > ichiCheck.SenkouSpanB)
                        {
                            model.priceCompareIchi = 3;
                        }
                        else
                        {
                            model.priceCompareIchi = 2;
                        }
                        lReport.Add(model);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.WriteLine($"{dem++}. {item.s}");
                }

                var count = lReport.Count;
                //Tỉ lệ cp trên ma20 
                strOutput.AppendLine($"[Thống kê PTKT]");
                strOutput.AppendLine($"- Số cp trên MA20: {Math.Round((float)lReport.Count(x => x.isGEMA20) * 100 / count, 1)}%");
                strOutput.AppendLine($"- Số cp tăng giá: {Math.Round((float)lReport.Count(x => x.isPriceUp) * 100 / count, 1)}%");
             

                var lBatDay = lReport.Where(x => x.bd)
                    //.OrderByDescending(x => x.isRsiZero)
                    .OrderBy(x => x.priceBB)
                    .ThenBy(x => x.rank)
                    .Take(20);
                if(lBatDay.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top 10 cp bắt đáy:");
                    var index = 1;
                    foreach (var item in lBatDay)
                    {
                        strOutput.Append($"{index++}.{item.s}");
                        if (item.isRsiZero)
                        {
                            strOutput.AppendLine(" - RSI zero");
                        }
                        else
                        {
                            strOutput.AppendLine();
                        }
                    }
                }    

                var lTrenMa20 = lReport.Where(x => x.isPriceUp && x.isCrossMa20Up)
                                    .OrderBy(x => x.rank)
                                    .Take(15);
                if (lTrenMa20.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top 10 cp cắt lên MA20:");
                    var index = 1;
                    foreach (var item in lTrenMa20)
                    {
                        strOutput.AppendLine($"{index++}.{item.s}");
                    }
                }
               
               
                var lTrenMa20Extend = lReport.Where(x => x.isPriceUp && x.isCrossMa20Up)
                                  .OrderByDescending(x => x.priceCompareIchi)
                                  .ThenBy(x => x.priceBB)
                                  .ThenBy(x => x.rank)
                                  .Take(15);
                if (lTrenMa20Extend.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top 10 cp quan tâm:");
                    var index = 1;
                    foreach (var item in lTrenMa20Extend)
                    {
                        strOutput.AppendLine($"{index++}.{item.s}");
                    }
                }
                var dtEnd = DateTime.Now;
                var ts = (dtEnd - dtStart).TotalSeconds;
                return (1, strOutput.ToString());
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return (0, null);
        }
    }

    public class ReportPTKT
    {
        public string s { get; set; }
        public int rank { get; set; }
        public bool bd { get; set; }//Chỉ báo bắt đáy
        public bool isCrossMa20Up { get; set; }// cắt lên và nến xanh
        public bool isGEMA20 { get; set; }//Nằm trên MA20
        public bool isRsiZero { get; set; }
        public int priceCompareIchi { get; set; }//Vị trí tương đối so với ichimoku - dưới(1), trong(2), trên(3)
        public int priceBB { get; set; }//Vị trí tương đối so với đường BB - dưới(1), trong(2), trên(3)
        public bool isPriceUp { get; set; }//cp tăng giá hay ko
    }
}

