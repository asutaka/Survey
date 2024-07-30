using Microsoft.Extensions.Logging;
using Skender.Stock.Indicators;
using SLib.Model;
using SLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task<ThongKeKhacModel> TinhToanThongKeDuaVaoDuLieu(Stock entity)
        {
            try
            {
                var lData = await _apiService.GetDataStock(entity.s);
                return TinhToanThongKeDuaVaoDuLieu(entity, lData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public ThongKeKhacModel TinhToanThongKeDuaVaoDuLieu(Stock entity, List<Quote> lData)
        {
            try
            {
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

                //var countBC = entity.bc.Count();
                //if (countBC > 3)
                //{
                //    //check dữ liệu báo cáo quý có tin tưởng hay ko
                //    var lDiv = new List<long>();
                //    for (int i = 0; i < countBC - 1; i++)
                //    {
                //        var elementCur = entity.bc[i].t;
                //        var elementNext = entity.bc[i + 1].t;
                //        lDiv.Add((elementCur - elementNext)/86400000);
                //    }
                //    if (!lDiv.Any(x => x > 180))
                //    {
                //        //here
                //    }


                //    //entity.bc.Reverse();
                //}
                //giacpTuDauQuyDenHienTai
                var last = lData.Last();
                var quarter = dt.GetQuarter();
                DateTime dtQuarter;
                switch (quarter)
                {
                    case 1: dtQuarter = new DateTime(dt.Year, 1, 1); break;
                    case 2: dtQuarter = new DateTime(dt.Year, 4, 1); break;
                    case 3: dtQuarter = new DateTime(dt.Year, 7, 1); break;
                    default: dtQuarter = new DateTime(dt.Year, 10, 1); break;
                }
                var entityQuarter = lData.FirstOrDefault(x => x.Date.Year == dtQuarter.Year && x.Date.Month == dtQuarter.Month && x.Date.Day == dtQuarter.Day);
                model.giacpTuDauQuyDenHienTai = Math.Round(100 * (-1 + last.Close / entityQuarter.Close), 1);
                //giacpTangTB_Quy
                //var lDataQuy = lData.Where(x => x.Date < entityQuarter.Date)
                //    .GroupBy(x => new { x.Date.Year, x.Date.Month }, (key, group) => new
                //    {
                //        Year = key.Year,
                //        Month = key.Month,
                //        Open = group.First().Open,
                //        Close = group.Last().Close
                //    });
                //lDataQuy.Reverse();
                //var countDataQuy = lDataQuy.Count();
                //var lQuy = new List<decimal>();
                //for (int i = 0; i < countDataQuy; i = i + 3)
                //{
                //    var itemFirst = lDataQuy.ElementAt(i);
                //    var itemLast = lDataQuy.ElementAt(i + 2);
                //    var rateQuy = Math.Round(100 * (-1 + itemLast.Close / itemFirst.Open), 1);
                //    lQuy.Add(rateQuy);
                //    if (lQuy.Count() == 10)
                //        break;
                //}
                //model.giacpTangTB_Quy = lQuy.Average();

                /*
                    muabanTheoMa20, tilebreakMa20Loi: 
                    - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma) và nến ngay trước phải nằm toàn bộ thân nến phía dưới MA
                    - điều kiện bán: bán sau T3
                */
                var ma20 = CalculateMa(lMa20);
                model.muabanTheoMa20 = ma20.Item1;
                model.tilebreakMa20Loi = ma20.Item2;

                var ma20EX = CalculateMaEX(lMa20);
                model.muabanTheoMa20_EX = ma20EX.Item1;
                model.tilebreakMa20Loi_EX = ma20EX.Item2;
                /*
                   muabanTheoE10, tilebreakE10LoikMa20Loi: 
                   - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma) và nến ngay trước phải nằm toàn bộ thân nến phía dưới MA
                   - điều kiện bán: bán sau T3
               */
                var e10 = CalculateEma(lEma10);
                model.muabanTheoE10 = e10.Item1;
                model.tilebreakE10Loi = e10.Item2;

                var e10EX = CalculateEmaEX(lEma10);
                model.muabanTheoE10_EX = e10EX.Item1;
                model.tilebreakE10Loi_EX = e10EX.Item2;
                /*
                  muabanTheoE21, tilebreakE21Loi: 
                  - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma) và nến ngay trước phải nằm toàn bộ thân nến phía dưới MA
                  - điều kiện bán: bán sau T3
              */
                var e21 = CalculateEma(lEma21);
                model.muabanTheoE21 = e21.Item1;
                model.tilebreakE21Loi = e21.Item2;

                var e21EX = CalculateEmaEX(lEma21);
                model.muabanTheoE21_EX = e21EX.Item1;
                model.tilebreakE21Loi_EX = e21EX.Item2;
                /*
                 * Ichi
                   muabanTheoMa20, tilebreakMa20Loi: 
                   - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma) và nến ngay trước phải nằm toàn bộ thân nến phía dưới MA
                   - điều kiện bán: bán sau T3
               */
                var ma20Ichi = CalculateMaIchi(lMa20);
                model.muabanTheoMa20_Ichi = ma20Ichi.Item1;
                model.tilebreakMa20Loi_Ichi = ma20Ichi.Item2;

                var ma20IchiEX = CalculateMaIchiEX(lMa20);
                model.muabanTheoMa20_Ichi_EX = ma20IchiEX.Item1;
                model.tilebreakMa20Loi_Ichi_EX = ma20IchiEX.Item2;
                /*
                 * Ichi
                   muabanTheoE10, tilebreakE10LoikMa20Loi: 
                   - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma) và nến ngay trước phải nằm toàn bộ thân nến phía dưới MA
                   - điều kiện bán: bán sau T3
               */
                var e10Ichi = CalculateEmaIchi(lEma10);
                model.muabanTheoE10_Ichi = e10Ichi.Item1;
                model.tilebreakE10Loi_Ichi = e10Ichi.Item2;

                var e10IchiEX = CalculateEmaIchiEX(lEma10);
                model.muabanTheoE10_Ichi_EX = e10IchiEX.Item1;
                model.tilebreakE10Loi_Ichi_EX = e10IchiEX.Item2;
                /*
                 * Ichi
                  muabanTheoE21, tilebreakE21Loi: 
                  - điều kiện mua: mua khi nến cắt lên MA20 và xanh(O =< Ma và C >= Ma) và nến ngay trước phải nằm toàn bộ thân nến phía dưới MA
                  - điều kiện bán: bán sau T3
              */
                var e21Ichi = CalculateEmaIchi(lEma21);
                model.muabanTheoE21_Ichi = e21Ichi.Item1;
                model.tilebreakE21Loi_Ichi = e21Ichi.Item2;

                var e21IchiEX = CalculateEmaIchiEX(lEma21);
                model.muabanTheoE21_Ichi_EX = e21IchiEX.Item1;
                model.tilebreakE21Loi_Ichi_EX = e21IchiEX.Item2;

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
                            if (item.Low < (decimal)itemInput.Sma && item.Close >= (decimal)itemInput.Sma)
                            {
                                var prev = lData.ElementAt(i - 1);
                                var inputPrev = lInput.ElementAt(i - 1);
                                if (prev.Open < (decimal)(inputPrev.Sma ?? 0) && prev.Close < (decimal)(inputPrev.Sma ?? 0))
                                {
                                    isBuy = true;
                                    priceBuy = item.Close;
                                    cd++;
                                    //_logger.LogError($"Ngay mua: {item.Date.ToString("dd/MM/yyyy")}; Gia mua: {priceBuy}");
                                }
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
                            //_logger.LogError($"             =>Ngay ban: {item.Date.ToString("dd/MM/yyyy")}; Gia ban: {item.Close}; Ti le: {lRate.Last()}%"); 
                            cd = 0;
                            priceBuy = 0;
                            isBuy = false;
                        }
                    }
                    return (Math.Round(lRate.Average(), 1), Math.Round((decimal)100 * lRate.Count(x => x <= 0) / lRate.Count(), 1));
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
                                var prev = lData.ElementAt(i - 1);
                                var inputPrev = lInput.ElementAt(i - 1);
                                if (prev.Open < (decimal)(inputPrev.Ema ?? 0) && prev.Close < (decimal)(inputPrev.Ema ?? 0))
                                {
                                    isBuy = true;
                                    priceBuy = item.Close;
                                    cd++;
                                    //_logger.LogError($"Ngay mua: {item.Date.ToString("dd/MM/yyyy")}; Gia mua: {priceBuy}");
                                }
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
                            if (item.Open <= (decimal)itemInput.Sma && item.Close >= (decimal)itemInput.Sma && item.Close >= Math.Max(itemIchi.SenkouSpanA ?? 0, itemIchi.SenkouSpanB ?? 0))
                            {
                                var prev = lData.ElementAt(i - 1);
                                var inputPrev = lInput.ElementAt(i - 1);
                                if (prev.Open < (decimal)(inputPrev.Sma ?? 0) && prev.Close < (decimal)(inputPrev.Sma ?? 0))
                                {
                                    isBuy = true;
                                    priceBuy = item.Close;
                                    cd++;
                                    //_logger.LogError($"Ngay mua: {item.Date.ToString("dd/MM/yyyy")}; Gia mua: {priceBuy}");
                                }
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
                            //_logger.LogError($"             =>Ngay ban: {item.Date.ToString("dd/MM/yyyy")}; Gia ban: {item.Close}; Ti le: {lRate.Last()}%");
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
                                var prev = lData.ElementAt(i - 1);
                                var inputPrev = lInput.ElementAt(i - 1);
                                if (prev.Open < (decimal)(inputPrev.Ema ?? 0) && prev.Close < (decimal)(inputPrev.Ema ?? 0))
                                {
                                    isBuy = true;
                                    priceBuy = item.Close;
                                    cd++;
                                    //_logger.LogError($"Ngay mua: {item.Date.ToString("dd/MM/yyyy")}; Gia mua: {priceBuy}");
                                }
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

                (decimal, decimal) CalculateMaEX(IEnumerable<SmaResult> lInput)
                {
                    var isBuy = false;
                    var lRate = new List<decimal>();
                    decimal priceBuy = 0;
                    var cd = 0;
                    var isBelowIchi = false;
                    for (int i = 0; i < count; i++)
                    {
                        var item = lData.ElementAt(i);
                        var itemInput = lInput.ElementAt(i);
                        if (itemInput.Sma is null)
                            continue;

                        if (!isBuy)
                        {
                            cd = 0;
                            if (item.Low < (decimal)itemInput.Sma && item.Close >= (decimal)itemInput.Sma)
                            {
                                var prev = lData.ElementAt(i - 1);
                                var inputPrev = lInput.ElementAt(i - 1);
                                if (prev.Open < (decimal)(inputPrev.Sma ?? 0) && prev.Close < (decimal)(inputPrev.Sma ?? 0))
                                {
                                    var ichi = lIchi.ElementAt(i);
                                    isBuy = true;
                                    priceBuy = item.Close;
                                    if (Math.Max(ichi.SenkouSpanA ?? 0, ichi.SenkouSpanB ?? 0) > priceBuy)
                                    {
                                        isBelowIchi = true;
                                    }
                                    cd++;
                                    //_logger.LogError($"Ngay mua: {item.Date.ToString("dd/MM/yyyy")}; Gia mua: {priceBuy}");
                                }
                            }
                            continue;
                        }

                        if (cd < 3)
                        {
                            cd++;
                            continue;
                        }
                        //Nếu điểm mua dưới ichi thì chỉ cần close đạt 10% mà chưa vượt được mây là bán
                        if (isBelowIchi)
                        {
                            var rate = Math.Round(-1 + item.Close / priceBuy, 1);
                            if (rate >= (decimal)0.1)
                            {
                                var ichi = lIchi.ElementAt(i);
                                if (Math.Max(ichi.SenkouSpanA ?? 0, ichi.SenkouSpanB ?? 0) > item.Close)
                                {
                                    lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                                    //_logger.LogError($"             =>Ngay ban: {item.Date.ToString("dd/MM/yyyy")}; Gia ban: {item.Close}; Ti le: {lRate.Last()}%");
                                    cd = 0;
                                    priceBuy = 0;
                                    isBuy = false;
                                    isBelowIchi = false;
                                    continue;
                                }
                            }

                        }

                        if (item.Close < (decimal)itemInput.Sma)
                        {
                            lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                            //_logger.LogError($"             =>Ngay ban: {item.Date.ToString("dd/MM/yyyy")}; Gia ban: {item.Close}; Ti le: {lRate.Last()}%");
                            cd = 0;
                            priceBuy = 0;
                            isBuy = false;
                            isBelowIchi = false;
                        }
                    }
                    return (Math.Round(lRate.Average(), 1), Math.Round((decimal)100 * lRate.Count(x => x <= 0) / lRate.Count(), 1));
                }
                (decimal, decimal) CalculateEmaEX(IEnumerable<EmaResult> lInput)
                {
                    var isBuy = false;
                    var lRate = new List<decimal>();
                    decimal priceBuy = 0;
                    var cd = 0;
                    var isBelowIchi = false;
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
                                var prev = lData.ElementAt(i - 1);
                                var inputPrev = lInput.ElementAt(i - 1);
                                if (prev.Open < (decimal)(inputPrev.Ema ?? 0) && prev.Close < (decimal)(inputPrev.Ema ?? 0))
                                {
                                    var ichi = lIchi.ElementAt(i);
                                    isBuy = true;
                                    priceBuy = item.Close;
                                    if (Math.Max(ichi.SenkouSpanA ?? 0, ichi.SenkouSpanB ?? 0) > priceBuy)
                                    {
                                        isBelowIchi = true;
                                    }
                                    cd++;
                                    //_logger.LogError($"Ngay mua: {item.Date.ToString("dd/MM/yyyy")}; Gia mua: {priceBuy}");
                                }
                            }
                            continue;
                        }

                        if (cd < 3)
                        {
                            cd++;
                            continue;
                        }

                        //Nếu điểm mua dưới ichi thì chỉ cần close đạt 10% mà chưa vượt được mây là bán
                        if (isBelowIchi)
                        {
                            var rate = Math.Round(-1 + item.Close / priceBuy, 1);
                            if (rate >= (decimal)0.1)
                            {
                                var ichi = lIchi.ElementAt(i);
                                if (Math.Max(ichi.SenkouSpanA ?? 0, ichi.SenkouSpanB ?? 0) > item.Close)
                                {
                                    lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                                    //_logger.LogError($"             =>Ngay ban: {item.Date.ToString("dd/MM/yyyy")}; Gia ban: {item.Close}; Ti le: {lRate.Last()}%");
                                    cd = 0;
                                    priceBuy = 0;
                                    isBuy = false;
                                    isBelowIchi = false;
                                    continue;
                                }
                            }

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
                (decimal, decimal) CalculateMaIchiEX(IEnumerable<SmaResult> lInput)
                {
                    var isBuy = false;
                    var lRate = new List<decimal>();
                    decimal priceBuy = 0;
                    var cd = 0;
                    var isBelowIchi = false;
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
                            if (item.Open <= (decimal)itemInput.Sma && item.Close >= (decimal)itemInput.Sma && item.Close >= Math.Max(itemIchi.SenkouSpanA ?? 0, itemIchi.SenkouSpanB ?? 0))
                            {
                                var prev = lData.ElementAt(i - 1);
                                var inputPrev = lInput.ElementAt(i - 1);
                                if (prev.Open < (decimal)(inputPrev.Sma ?? 0) && prev.Close < (decimal)(inputPrev.Sma ?? 0))
                                {
                                    var ichi = lIchi.ElementAt(i);
                                    isBuy = true;
                                    priceBuy = item.Close;
                                    if (Math.Max(ichi.SenkouSpanA ?? 0, ichi.SenkouSpanB ?? 0) > priceBuy)
                                    {
                                        isBelowIchi = true;
                                    }
                                    cd++;
                                    //_logger.LogError($"Ngay mua: {item.Date.ToString("dd/MM/yyyy")}; Gia mua: {priceBuy}");
                                }
                            }
                            continue;
                        }

                        if (cd < 3)
                        {
                            cd++;
                            continue;
                        }

                        //Nếu điểm mua dưới ichi thì chỉ cần close đạt 10% mà chưa vượt được mây là bán
                        if (isBelowIchi)
                        {
                            var rate = Math.Round(-1 + item.Close / priceBuy, 1);
                            if (rate >= (decimal)0.1)
                            {
                                var ichi = lIchi.ElementAt(i);
                                if (Math.Max(ichi.SenkouSpanA ?? 0, ichi.SenkouSpanB ?? 0) > item.Close)
                                {
                                    lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                                    //_logger.LogError($"             =>Ngay ban: {item.Date.ToString("dd/MM/yyyy")}; Gia ban: {item.Close}; Ti le: {lRate.Last()}%");
                                    cd = 0;
                                    priceBuy = 0;
                                    isBuy = false;
                                    isBelowIchi = false;
                                    continue;
                                }
                            }

                        }

                        if (item.Close < (decimal)itemInput.Sma)
                        {
                            lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                            //_logger.LogError($"             =>Ngay ban: {item.Date.ToString("dd/MM/yyyy")}; Gia ban: {item.Close}; Ti le: {lRate.Last()}%");
                            cd = 0;
                            priceBuy = 0;
                            isBuy = false;
                        }
                    }
                    return (Math.Round(lRate.Average(), 1), Math.Round((decimal)100 * lRate.Count(x => x <= 0) / lRate.Count(), 1));
                }
                (decimal, decimal) CalculateEmaIchiEX(IEnumerable<EmaResult> lInput)
                {
                    var isBuy = false;
                    var lRate = new List<decimal>();
                    decimal priceBuy = 0;
                    var cd = 0;
                    var isBelowIchi = false;
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
                                var prev = lData.ElementAt(i - 1);
                                var inputPrev = lInput.ElementAt(i - 1);
                                if (prev.Open < (decimal)(inputPrev.Ema ?? 0) && prev.Close < (decimal)(inputPrev.Ema ?? 0))
                                {
                                    var ichi = lIchi.ElementAt(i);
                                    isBuy = true;
                                    priceBuy = item.Close;
                                    if (Math.Max(ichi.SenkouSpanA ?? 0, ichi.SenkouSpanB ?? 0) > priceBuy)
                                    {
                                        isBelowIchi = true;
                                    }
                                    cd++;
                                    //_logger.LogError($"Ngay mua: {item.Date.ToString("dd/MM/yyyy")}; Gia mua: {priceBuy}");
                                }
                            }
                            continue;
                        }

                        if (cd < 3)
                        {
                            cd++;
                            continue;
                        }

                        //Nếu điểm mua dưới ichi thì chỉ cần close đạt 10% mà chưa vượt được mây là bán
                        if (isBelowIchi)
                        {
                            var rate = Math.Round(-1 + item.Close / priceBuy, 1);
                            if (rate >= (decimal)0.1)
                            {
                                var ichi = lIchi.ElementAt(i);
                                if (Math.Max(ichi.SenkouSpanA ?? 0, ichi.SenkouSpanB ?? 0) > item.Close)
                                {
                                    lRate.Add(Math.Round(100 * (-1 + item.Close / priceBuy), 1));
                                    //_logger.LogError($"             =>Ngay ban: {item.Date.ToString("dd/MM/yyyy")}; Gia ban: {item.Close}; Ti le: {lRate.Last()}%");
                                    cd = 0;
                                    priceBuy = 0;
                                    isBuy = false;
                                    isBelowIchi = false;
                                    continue;
                                }
                            }

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
                _logger.LogError(ex.Message);
            }
            return null;
        }
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
        //

        public decimal muabanTheoMa20_EX { get; set; }
        public decimal tilebreakMa20Loi_EX { get; set; }
        public decimal muabanTheoE10_EX { get; set; }
        public decimal tilebreakE10Loi_EX { get; set; }
        public decimal muabanTheoE21_EX { get; set; }
        public decimal tilebreakE21Loi_EX { get; set; }
        //Ichi
        public decimal muabanTheoMa20_Ichi_EX { get; set; }
        public decimal tilebreakMa20Loi_Ichi_EX { get; set; }
        public decimal muabanTheoE10_Ichi_EX { get; set; }
        public decimal tilebreakE10Loi_Ichi_EX { get; set; }
        public decimal muabanTheoE21_Ichi_EX { get; set; }
        public decimal tilebreakE21Loi_Ichi_EX { get; set; }
    }
}
