using MongoDB.Driver;
using Skender.Stock.Indicators;
using SLib.Model;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<(int, List<string>)> ChiBaoKyThuat()
        {
            try
            {
                var dt = DateTime.Now;
                var t = long.Parse($"{dt.Year}{dt.Month.To2Digit()}{dt.Day.To2Digit()}");
                var builderConfig = Builders<ConfigData>.Filter;
                FilterDefinition<ConfigData> filterConfig = builderConfig.Eq(x => x.ty, (int)EConfigDataType.ChiBaoKyThuat);
                var lConfig = _configRepo.GetByFilter(filterConfig);
                if (lConfig.Any())
                {
                    if (lConfig.Any(x => x.t == t))
                        return (0, null);

                    _configRepo.DeleteMany(filterConfig);
                }

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
                        var model = await ChiBaoKyThuatOnlyStock(item.s, item.rank, 50000);
                        if (model is null)
                            continue;

                        lReport.Add(model);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.WriteLine($"{dem++}. {item.s}");
                }

                var lMes = new List<string>();
                var count = lReport.Count;
                //Tỉ lệ cp trên ma20 
                strOutput.AppendLine($"[Thống kê PTKT]");
                strOutput.AppendLine($"- Số cp trên MA20: {Math.Round((float)lReport.Count(x => x.isGEMA20) * 100 / count, 1)}%");
                strOutput.AppendLine($"- Số cp tăng giá: {Math.Round((float)lReport.Count(x => x.isPriceUp) * 100 / count, 1)}%");
             

                var lBatDay = lReport.Where(x => x.bd)
                    .OrderBy(x => x.rank)
                    .ThenBy(x => x.priceBB)
                    .Take(20);
                if(lBatDay.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top 20 cp bắt đáy:");
                    var index = 1;
                    foreach (var item in lBatDay)
                    {
                        strOutput.Append($"{index++}. {item.s}");
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
                                    .Take(20);
                if (lTrenMa20.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top 20 cp cắt lên MA20:");
                    var index = 1;
                    foreach (var item in lTrenMa20)
                    {
                        var content = $"{index++}. {item.s}";
                        if(item.isIchi)
                        {
                            content += " - Ichimoku";
                        }
                        strOutput.AppendLine(content);
                    }
                }

                lMes.Add(strOutput.ToString());
                //
                strOutput.Clear();
                strOutput.AppendLine("[Dan Zanger]");

                var lTrenEma21 = lReport.Where(x => x.isPriceUp && x.isCrossEma21Up)
                            .OrderBy(x => x.rank)
                            .Take(20);
                if (lTrenEma21.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top 20 cp cắt lên E21:");
                    var index = 1;
                    foreach (var item in lTrenEma21)
                    {
                        var content = $"{index++}. {item.s}";
                        if (item.isIchi)
                        {
                            content += " - Ichimoku";
                        }
                        strOutput.AppendLine(content);
                    }
                }

                var lTrenEma50 = lReport.Where(x => x.isPriceUp && x.isCrossEma50Up)
                            .OrderBy(x => x.rank)
                            .Take(20);
                if (lTrenEma50.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top 20 cp cắt lên E50:");
                    var index = 1;
                    foreach (var item in lTrenEma50)
                    {
                        var content = $"{index++}. {item.s}";
                        if (item.isIchi)
                        {
                            content += " - Ichimoku";
                        }
                        strOutput.AppendLine(content);
                    }
                }

                var lEma21_50 = lReport.Where(x => x.isPriceUp && x.isEma21_50)
                           .OrderBy(x => x.rank)
                           .Take(20);
                if (lEma21_50.Any())
                {
                    strOutput.AppendLine();
                    strOutput.AppendLine($"*Top 20 cp E21 cắt E50:");
                    var index = 1;
                    foreach (var item in lEma21_50)
                    {
                        var content = $"{index++}. {item.s}";
                        if (item.isIchi)
                        {
                            content += " - Ichimoku";
                        }
                        strOutput.AppendLine(content);
                    }
                }

                lMes.Add(strOutput.ToString());

                var dtEnd = DateTime.Now;
                var ts = (dtEnd - dtStart).TotalSeconds;

                _configRepo.InsertOne(new ConfigData
                {
                    ty = (int)EConfigDataType.ChiBaoKyThuat,
                    t = t
                });

                return (1, lMes);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return (0, null);
        }
        private async Task<ReportPTKT> ChiBaoKyThuatOnlyStock(string code, int rank, int limitvol)
        {
            try
            {
                var lData = await _apiService.GetDataStock(code);
                if (lData.Count() < 250)
                    return null;
                if (limitvol > 0 && lData.Last().Volume < limitvol)
                    return null;

                var model = new ReportPTKT
                {
                    s = code,
                    rank = rank
                };

                var lIchi = lData.GetIchimoku();
                var lBb = lData.GetBollingerBands();
                var lEma21 = lData.GetEma(21);
                var lEma50 = lData.GetEma(50);
                //var lEma200 = lData.GetEma(200);
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
                var entityNear = lData.SkipLast(1).TakeLast(1).First();
                var bbNear = lBb.SkipLast(1).TakeLast(1).First();

                model.curPrice = entity.Close;
                model.basicPrice = entityNear.Close;
                model.isGEMA20 = entity.Close >= (decimal)bb.Sma;
                model.isCrossMa20Up = entityNear.Close < (decimal)bbNear.Sma && entity.Close >= (decimal)bb.Sma && entity.Open <= (decimal)bb.Sma;
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
                if (entity.Close > ichiCheck.SenkouSpanA && entity.Close > ichiCheck.SenkouSpanB)
                {
                    model.isIchi = true;
                }
                //EMA21
                var ema21 = lEma21.Last();
                var ema50 = lEma50.Last();
                var ema21Near = lEma21.SkipLast(1).TakeLast(1).First();
                var ema50Near = lEma50.SkipLast(1).TakeLast(1).First();

                if (entityNear.Close < (decimal)ema21Near.Ema
                    && entity.Close >= (decimal)ema21.Ema
                    && entity.Open <= (decimal)ema21.Ema)
                {
                    model.isCrossEma21Up = true;
                }
                if (entityNear.Close < (decimal)ema50Near.Ema
                    && entity.Close >= (decimal)ema50.Ema
                    && entity.Open <= (decimal)ema50.Ema)
                {
                    model.isCrossEma50Up = true;
                }
                if (ema21Near.Ema < ema50Near.Ema
                    && ema21.Ema >= ema50.Ema)
                {
                    model.isEma21_50 = true;
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<ThongKeKhacModel> TinhToanThongKeDuaVaoDuLieu(string code)
        {
            try
            {
                var lData = await _apiService.GetDataStock(code);
                var count = lData.Count;
                if (count < 250)
                    return null;
                var dt = DateTime.Now;
                var model = new ThongKeKhacModel();

                var lIchi = lData.GetIchimoku();
                var lBb = lData.GetBollingerBands();
                var lEma21 = lData.GetEma(21);
                var lEma50 = lData.GetEma(50);
                var lEma10 = lData.GetEma(10);
                var lMa20 = lData.GetSma(20);
                var lRsi = lData.GetRsi();

                //giacpTuDauQuyDenHienTai
                var last = lData.Last();
                var quarter = dt.GetQuarter();
                DateTime dtQuarter;
                switch (quarter)
                {
                    case 1: dtQuarter = new DateTime(dt.Year, 1, 1);break;
                    case 2: dtQuarter = new DateTime(dt.Year, 4, 1);break;
                    case 3: dtQuarter = new DateTime(dt.Year, 7, 1);break;
                    default: dtQuarter = new DateTime(dt.Year, 10, 1); break;
                }
                var entityQuarter = lData.FirstOrDefault(x => x.Date.Year == dtQuarter.Year && x.Date.Month == dtQuarter.Month && x.Date.Day == dtQuarter.Day);
                model.giacpTuDauQuyDenHienTai = Math.Round(100 * (-1 + last.Close / entityQuarter.Close), 1);
                //giacpTangTB_Quy
                var lDataQuy = lData.Where(x => x.Date < entityQuarter.Date)
                    .GroupBy(x => new { x.Date.Year, x.Date.Month }, (key, group) => new
                    {
                        Year = key.Year,
                        Month = key.Month,
                        Open = group.First().Open,
                        Close = group.Last().Close
                    });
                lDataQuy.Reverse();
                var countDataQuy = lDataQuy.Count();
                var lQuy = new List<decimal>();
                for (int i = 0; i < countDataQuy; i = i+3)
                {
                    var itemFirst = lDataQuy.ElementAt(i);
                    var itemLast = lDataQuy.ElementAt(i+2);
                    var rateQuy = Math.Round(100 * (-1 + itemLast.Close / itemFirst.Open), 1);
                    lQuy.Add(rateQuy);
                    if (lQuy.Count() == 10)
                        break;
                }
                model.giacpTangTB_Quy = lQuy.Average();
                /*
                    muabanTheoMa20, tilebreakMa20Loi: 
                    - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma)
                    - điều kiện bán: bán sau T3
                */
                var ma20 = CalculateMa(lMa20);
                model.muabanTheoMa20 = ma20.Item1;
                model.tilebreakMa20Loi = ma20.Item2;
                /*
                   muabanTheoE10, tilebreakE10LoikMa20Loi: 
                   - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma)
                   - điều kiện bán: bán sau T3
               */
                var e10 = CalculateEma(lEma10);
                model.muabanTheoE10 = e10.Item1;
                model.tilebreakE10Loi = e10.Item2;
                /*
                  muabanTheoE21, tilebreakE21Loi: 
                  - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma)
                  - điều kiện bán: bán sau T3
              */
                var e21 = CalculateEma(lEma21);
                model.muabanTheoE21 = e21.Item1;
                model.tilebreakE21Loi = e21.Item2;

                /*
                 * Ichi
                   muabanTheoMa20, tilebreakMa20Loi: 
                   - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma)
                   - điều kiện bán: bán sau T3
               */
                var ma20Ichi = CalculateMaIchi(lMa20);
                model.muabanTheoMa20_Ichi = ma20Ichi.Item1;
                model.tilebreakMa20Loi_Ichi = ma20Ichi.Item2;
                /*
                 * Ichi
                   muabanTheoE10, tilebreakE10LoikMa20Loi: 
                   - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma)
                   - điều kiện bán: bán sau T3
               */
                var e10Ichi = CalculateEmaIchi(lEma10);
                model.muabanTheoE10_Ichi = e10Ichi.Item1;
                model.tilebreakE10Loi_Ichi = e10Ichi.Item2;
                /*
                 * Ichi
                  muabanTheoE21, tilebreakE21Loi: 
                  - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma)
                  - điều kiện bán: bán sau T3
              */
                var e21Ichi = CalculateEmaIchi(lEma21);
                model.muabanTheoE21_Ichi = e21Ichi.Item1;
                model.tilebreakE21Loi_Ichi = e21Ichi.Item2;

                (decimal, decimal) CalculateMa(IEnumerable<SmaResult> lInput)
                {
                    var isBuy = false;
                    var lRate = new List<decimal>();
                    decimal priceBuy = 0;
                    var cd = 0;
                    for (int i = 0; i < count; i++)
                    {
                        var item = lData.ElementAt(i);
                        var itemInput = lInput.ElementAt(i);
                        if (itemInput.Sma is null)
                            continue;

                        if (!isBuy)
                        {
                            cd = 0;
                            if (item.Open <= (decimal)itemInput.Sma && item.Close >= (decimal)itemInput.Sma)
                            {
                                isBuy = true;
                                priceBuy = item.Close;
                                cd++;
                            }
                            continue;
                        }

                        if (cd < 3)
                        {
                            cd++;
                            continue;
                        }

                        if (item.Close < (decimal)itemInput.Sma)
                        {
                            lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                            cd = 0;
                            priceBuy = 0;
                            isBuy = false;
                        }
                    }
                    return (Math.Round(lRate.Average(),1), Math.Round((decimal)100 * lRate.Count(x => x <= 0)/lRate.Count(),1));
                }
                (decimal, decimal) CalculateEma(IEnumerable<EmaResult> lInput)
                {
                    var isBuy = false;
                    var lRate = new List<decimal>();
                    decimal priceBuy = 0;
                    var cd = 0;
                    for (int i = 0; i < count; i++)
                    {
                        var item = lData.ElementAt(i);
                        var itemInput = lInput.ElementAt(i);
                        if (itemInput.Ema is null)
                            continue;

                        if (!isBuy)
                        {
                            cd = 0;
                            if (item.Open <= (decimal)itemInput.Ema && item.Close >= (decimal)itemInput.Ema)
                            {
                                isBuy = true;
                                priceBuy = item.Close;
                                cd++;
                            }
                            continue;
                        }

                        if (cd < 3)
                        {
                            cd++;
                            continue;
                        }

                        if (item.Close < (decimal)itemInput.Ema)
                        {
                            lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                            cd = 0;
                            priceBuy = 0;
                            isBuy = false;
                        }
                    }
                    return (Math.Round(lRate.Average(), 1), Math.Round((decimal)100 * lRate.Count(x => x <= 0) / lRate.Count(), 1));
                }
                (decimal, decimal) CalculateMaIchi(IEnumerable<SmaResult> lInput)
                {
                    var isBuy = false;
                    var lRate = new List<decimal>();
                    decimal priceBuy = 0;
                    var cd = 0;
                    for (int i = 0; i < count; i++)
                    {
                        var item = lData.ElementAt(i);
                        var itemInput = lInput.ElementAt(i);
                        if (itemInput.Sma is null)
                            continue;
                        var itemIchi = lIchi.ElementAt(i);
                        if (itemIchi.SenkouSpanA is null || itemIchi.SenkouSpanB is null)
                            continue;

                        if (!isBuy)
                        {
                            cd = 0;
                            if (item.Open <= (decimal)itemInput.Sma && item.Close >= (decimal)itemInput.Sma && item.Close >= Math.Max(itemIchi.SenkouSpanA??0, itemIchi.SenkouSpanB??0))
                            {
                                isBuy = true;
                                priceBuy = item.Close;
                                cd++;
                            }
                            continue;
                        }

                        if (cd < 3)
                        {
                            cd++;
                            continue;
                        }

                        if (item.Close < (decimal)itemInput.Sma)
                        {
                            lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                            cd = 0;
                            priceBuy = 0;
                            isBuy = false;
                        }
                    }
                    return (Math.Round(lRate.Average(), 1), Math.Round((decimal)100 * lRate.Count(x => x <= 0) / lRate.Count(), 1));
                }
                (decimal, decimal) CalculateEmaIchi(IEnumerable<EmaResult> lInput)
                {
                    var isBuy = false;
                    var lRate = new List<decimal>();
                    decimal priceBuy = 0;
                    var cd = 0;
                    for (int i = 0; i < count; i++)
                    {
                        var item = lData.ElementAt(i);
                        var itemInput = lInput.ElementAt(i);
                        if (itemInput.Ema is null)
                            continue;
                        var itemIchi = lIchi.ElementAt(i);
                        if (itemIchi.SenkouSpanA is null || itemIchi.SenkouSpanB is null)
                            continue;

                        if (!isBuy)
                        {
                            cd = 0;
                            if (item.Open <= (decimal)itemInput.Ema && item.Close >= (decimal)itemInput.Ema && item.Close >= Math.Max(itemIchi.SenkouSpanA ?? 0, itemIchi.SenkouSpanB ?? 0))
                            {
                                isBuy = true;
                                priceBuy = item.Close;
                                cd++;
                            }
                            continue;
                        }

                        if (cd < 3)
                        {
                            cd++;
                            continue;
                        }

                        if (item.Close < (decimal)itemInput.Ema)
                        {
                            lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                            cd = 0;
                            priceBuy = 0;
                            isBuy = false;
                        }
                    }
                    return (Math.Round(lRate.Average(), 1), Math.Round((decimal)100 * lRate.Count(x => x <= 0) / lRate.Count(), 1));
                }
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }

    public class ReportPTKT
    {
        public string s { get; set; }
        public int rank { get; set; }
        public decimal curPrice { get; set; }
        public decimal basicPrice { get; set; }
        public bool bd { get; set; }//Chỉ báo bắt đáy
        public bool isCrossMa20Up { get; set; }// cắt lên và nến xanh
        public bool isGEMA20 { get; set; }//Nằm trên MA20
        public bool isRsiZero { get; set; }
        public bool isIchi { get; set; }//giá vượt trên ichi
        public int priceBB { get; set; }//Vị trí tương đối so với đường BB - dưới(1), trong(2), trên(3)
        public bool isPriceUp { get; set; }//cp tăng giá hay ko

        public bool isCrossEma21Up { get; set; }
        public bool isCrossEma50Up { get; set; }
        public bool isEma21_50 { get; set; }
    }

    public class ThongKeKhacModel
    {
        public decimal giacpTuDauQuyDenHienTai { get; set; }
        public decimal giacpTangTB_Quy { get; set; }
        public decimal muabanTheoMa20 { get; set; }
        public decimal tilebreakMa20Loi { get; set; }
        public decimal muabanTheoE10 { get; set; }
        public decimal tilebreakE10Loi { get; set; }
        public decimal muabanTheoE21 { get; set; }
        public decimal tilebreakE21Loi { get; set; }
        //Ichi
        public decimal muabanTheoMa20_Ichi { get; set; }
        public decimal tilebreakMa20Loi_Ichi { get; set; }
        public decimal muabanTheoE10_Ichi { get; set; }
        public decimal tilebreakE10Loi_Ichi { get; set; }
        public decimal muabanTheoE21_Ichi { get; set; }
        public decimal tilebreakE21Loi_Ichi { get; set; }
    }
}

