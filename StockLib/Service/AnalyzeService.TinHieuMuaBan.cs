﻿using Microsoft.Extensions.Logging;
using StockLib.DAL.Entity;
using StockLib.Utils;
using System.Text;

namespace StockLib.Service
{
    public partial class AnalyzeService
    {
        public async Task<(int, string)> TinHieuMuaBan()
        {
            try
            {
                var lDanCustom = new List<Stock>();
                var lGoldFish = new List<Stock>();
                var lSuperTrend = new List<Stock>();
                var lSuperTrendPhrase2 = new List<Stock>();
                var lEliot = new List<Stock>();
                var lEliot2 = new List<Stock>();

                foreach (var item in StaticVal._lStock)
                {
                    if (item.indicator is null
                        || !item.indicator.Any())
                        continue;

                    var lData = await _apiService.SSI_GetDataStock(item.s);
                    decimal a = 10;
                    if (item.ex == (int)EExchange.HSX)
                    {
                        a = 7;
                    }
                    else if (item.ex == (int)EExchange.UPCOM)
                    {
                        a = 15;
                    }

                    if (item.indicator.Any(x => x.type == (int)EIndicator.DanZangerVolumne))
                    {
                        var isDanCustomSignal = lData.CheckDanZangerCustom(a);
                        if (isDanCustomSignal)
                        {
                            lDanCustom.Add(item);
                        }
                    }

                    //if (item.indicator.Any(x => x.type == (int)EIndicator.GoldFish))
                    //{
                    //    var isGoldFish = lData.CheckGoldFishBuy();
                    //    if (isGoldFish)
                    //    {
                    //        lGoldFish.Add(item);
                    //    }
                    //}

                    if (item.indicator.Any(x => x.type == (int)EIndicator.SuperTrend))
                    {
                        var isSuperTrend = lData.CheckSuperTrend();
                        if (isSuperTrend)
                        {
                            lSuperTrend.Add(item);
                        }
                    }


                    if (item.indicator.Any(x => x.type == (int)EIndicator.SuperTrendPhrase2))
                    {
                        var isSuperTrendPhrase2 = lData.CheckSuperTrendPharse2();
                        if (isSuperTrendPhrase2)
                        {
                            lSuperTrendPhrase2.Add(item);
                        }
                    }

                    if(true)//Eliot
                    {
                        var eliot = lData.CheckEliot();
                        if(eliot.Item1)
                        {
                            if(eliot.Item2 == 1)
                            {
                                lEliot.Add(item);
                            }
                            else
                            {
                                lEliot2.Add(item);
                            }
                        }
                    }    
                }
                
                var sBuilder = new StringBuilder();

                if (lEliot.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Tín hiệu Eliot]");

                    foreach (var item in lEliot.OrderByDescending(x => x.rank).Take(10))
                    {
                        sBuilder.AppendLine($"{item.s}");
                    }
                }

                if (lEliot2.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Tín hiệu Eliot 2]");

                    foreach (var item in lEliot2.OrderByDescending(x => x.rank).Take(10))
                    {
                        sBuilder.AppendLine($"{item.s}");
                    }
                }


                if (lDanCustom.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Tín hiệu Danz Volume]");

                    foreach (var item in lDanCustom.OrderByDescending(x => x.indicator.FirstOrDefault(x => x.type == (int)EIndicator.DanZangerVolumne).rank).Take(10))
                    {
                        var indicator = item.indicator.FirstOrDefault(x => x.type == (int)EIndicator.DanZangerVolumne);
                        sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg_rate}%| Win/Loss: {indicator.win_rate}%/{indicator.loss_rate}%)");
                    }
                }

                if (lGoldFish.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Tín hiệu GoldFish]");

                    foreach (var item in lGoldFish.OrderByDescending(x => x.indicator.FirstOrDefault(x => x.type == (int)EIndicator.GoldFish).rank).Take(10))
                    {
                        var indicator = item.indicator.FirstOrDefault(x => x.type == (int)EIndicator.GoldFish);
                        sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg_rate}%| Win/Loss: {indicator.win_rate}%/{indicator.loss_rate}%)");
                    }
                }

                if (lSuperTrend.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Tín hiệu SuperTrend]");

                    foreach (var item in lSuperTrend.OrderByDescending(x => x.indicator.FirstOrDefault(x => x.type == (int)EIndicator.SuperTrend).rank).Take(10))
                    {
                        var indicator = item.indicator.FirstOrDefault(x => x.type == (int)EIndicator.SuperTrend);
                        sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg_rate}%| Win/Loss: {indicator.win_rate}%/{indicator.loss_rate}%)");
                    }
                }

                if (lSuperTrendPhrase2.Any())
                {
                    sBuilder.AppendLine();
                    sBuilder.AppendLine("[Tín hiệu SuperTrend - Phrase 2]");

                    foreach (var item in lSuperTrendPhrase2.OrderByDescending(x => x.indicator.FirstOrDefault(x => x.type == (int)EIndicator.SuperTrendPhrase2).rank).Take(10))
                    {
                        var indicator = item.indicator.FirstOrDefault(x => x.type == (int)EIndicator.SuperTrendPhrase2);
                        sBuilder.AppendLine($"{item.s}(TP trung bình: {indicator.avg_rate}%| Win/Loss: {indicator.win_rate}%/{indicator.loss_rate}%)");
                    }
                }

                if (string.IsNullOrWhiteSpace(sBuilder.ToString()))
                {
                    return (0, null);
                }
                return (1, sBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AnalyzeService.TinHieuMuaBan|EXCEPTION| {ex.Message}");
            }

            return (0, null);
        }
    }
}
