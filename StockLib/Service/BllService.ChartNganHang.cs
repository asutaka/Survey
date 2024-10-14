using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        public async Task<List<Stream>> Chart_NganHang(IEnumerable<string> lInput)
        {
            var lOutput = new List<Stream>();

            var lNganHang = lInput.Take(15).ToList();
            if (!lNganHang.Contains("OCB"))
            {
                lNganHang.Add("OCB");
            }
            var time = GetCurrentTime();

            var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, time.Item1));
            if (!lFinancial.Any())
                return null;
            var streamTinDung = await Chart_NganHang_TangTruongTinDung_RoomTinDung(lNganHang, lFinancial);
            lOutput.Add(streamTinDung);

            var streamNoXau = await Chart_NganHang_NoXau(lNganHang, lFinancial);
            lOutput.Add(streamNoXau);

            var streamNim = await Chart_NganHang_NimCasaChiPhiVon(lNganHang, lFinancial);
            lOutput.Add(streamNim);

            return lOutput;
        }
        private async Task<Stream> Chart_NganHang_TangTruongTinDung_RoomTinDung(IEnumerable<string> lNganHang, List<Financial> lFinancial)
        {
            try
            {
                var time = GetCurrentTime();

                var lTangTruongTinDung = new List<double>();
                var lRoomTinDung = new List<double>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lTangTruongTinDung.Add(0);
                        lRoomTinDung.Add(0);
                        continue;
                    }

                    //tang truong tin dung, room tin dung
                    lTangTruongTinDung.Add(cur.credit_r ?? 0);
                    lRoomTinDung.Add(cur.room ?? 0);
                }

                var basicColumn = new HighchartTangTruongTinDung($"Tăng trưởng tín dụng Quý {time.Item3}/{time.Item2} (YoY)", lNganHang.ToList(), new List<HighChartSeries_TangTruongTinDung>
                {
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Room tín dụng",
                        type = "column",
                        data = lRoomTinDung,
                        color = "rgba(158, 159, 163, 0.5)",
                        pointPlacement = -0.2,
                        dataLabels = new HighChartDataLabel()
                    },
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Tăng trưởng tín dụng",
                        type = "column",
                        data = lTangTruongTinDung,
                        color = "#012060",
                        dataLabels = new HighChartDataLabel()
                    }
                });
                var strTitleYAxis = "(Đơn vị: %)";
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_TangTruongTinDung_RoomTinDung|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        private async Task<Stream> Chart_NganHang_NoXau(IEnumerable<string> lNganHang, List<Financial> lFinancial)
        {
            try
            {
                var time = GetCurrentTime();

                var yearPrev = time.Item2;
                var quarterPrev = time.Item3;
                if (time.Item3 > 1)
                {
                    quarterPrev--;
                }
                else
                {
                    quarterPrev = 4;
                    yearPrev--;
                }
                var lFinancialPrev = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.d, int.Parse($"{yearPrev}{quarterPrev}")));

                var lBaoPhu = new List<double>();
                var lTiLeNo = new List<double>();
                var lTangTruongNoNhom2 = new List<double>();
                var lTangTruongTrichLap = new List<double>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lBaoPhu.Add(0);
                        lTiLeNo.Add(0);
                        lTangTruongNoNhom2.Add(0);
                        lTangTruongTrichLap.Add(0);
                        continue;
                    }
                    double baophu = 0, tileNoxau = 0, tangTruongTrichLap = 0, tangTruongNoNhom2 = 0;
                    var noxau = cur.debt3 + cur.debt4 + cur.debt5;
                    if (noxau > 0)
                    {
                        baophu = Math.Round(100 * (cur.risk ?? 0) / noxau, 1);
                    }
                    if (cur.debt > 0)
                    {
                        tileNoxau = Math.Round((float)(100 * noxau) / cur.debt, 1);
                    }

                    var prev = lFinancialPrev.FirstOrDefault(x => x.s == item);
                    if (prev != null)
                    {
                        if (prev.risk > 0)
                        {
                            tangTruongTrichLap = Math.Round(100 * (-1 + (cur.risk ?? 0) / (prev.risk ?? 1)), 1);
                        }
                        if (prev.debt2 > 0)
                        {
                            tangTruongNoNhom2 = Math.Round(100 * (-1 + (float)cur.debt2 / prev.debt2), 1);
                        }
                    }

                    //tang truong tin dung, room tin dung
                    lBaoPhu.Add(baophu);
                    lTiLeNo.Add(tileNoxau);
                    lTangTruongNoNhom2.Add(tangTruongNoNhom2);
                    lTangTruongTrichLap.Add(tangTruongTrichLap);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                     new() {
                        data = lBaoPhu,
                        name = "Bao phủ nợ xấu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#012060"
                    },
                    new()
                    {
                        data = lTiLeNo,
                        name = "Tỉ lệ nợ xấu",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    },
                    new()
                    {
                        data = lTangTruongNoNhom2,
                        name = "Tăng trưởng nợ nhóm 2",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1
                    },
                    new()
                    {
                        data = lTangTruongTrichLap,
                        name = "Tăng trưởng trích lập",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "rgba(158, 159, 163, 0.5)",
                        yAxis = 1
                    }
                };
                return await Chart_BasicBase($"Nợ xấu Quý {time.Item3}/{time.Item2} (QoQoY)", lNganHang.ToList(), lSeries, "(Bao phủ nợ xấu: %)", "(Tăng trưởng: %)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_NoXau|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        private async Task<Stream> Chart_NganHang_NimCasaChiPhiVon(IEnumerable<string> lNganHang, List<Financial> lFinancial)
        {
            try
            {
                var time = GetCurrentTime();

                var lNim = new List<double>();
                var lCasa = new List<double>();
                var lCir = new List<double>();
                var lChiPhiVon = new List<double>();
                foreach (var item in lNganHang)
                {
                    var cur = lFinancial.FirstOrDefault(x => x.s == item);
                    if (cur is null)
                    {
                        lNim.Add(0);
                        lCasa.Add(0);
                        lCir.Add(0);
                        lChiPhiVon.Add(0);
                        continue;
                    }

                    //tang truong tin dung, room tin dung
                    lNim.Add(cur.nim_r ?? 0);
                    lCasa.Add(cur.casa_r ?? 0);
                    lCir.Add(cur.cir_r ?? 0);
                    lChiPhiVon.Add(cur.cost_r ?? 0);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lNim,
                        name = "NIM",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lCasa,
                        name = "CASA",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lCir,
                        name = "CIR",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lChiPhiVon,
                        name = "Tăng trưởng chi phí vốn",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "rgba(158, 159, 163, 0.5)",
                        yAxis = 1
                    }
                };
                return await Chart_BasicBase($"NIM, CASA, CIR, Chi phí vốn Quý {time.Item3}/{time.Item2}", lNganHang.ToList(), lSeries, "(NIM: %)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_NimCasaChiPhiVon|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<List<Stream>> Chart_NganHang(string code)
        {
            var lFinancial = _financialRepo.GetByFilter(Builders<Financial>.Filter.Eq(x => x.s, code));
            if (!lFinancial.Any())
                return null;

            var lOutput = new List<Stream>();

            lFinancial = lFinancial.OrderBy(x => x.d).ToList();
            var streamTangTruongTinDung = await Chart_NganHang_TangTruongTinDung(lFinancial, code);
            var streamNoXau = await Chart_NganHang_NoXau(lFinancial, code);
            var streamNim = await Chart_NganHang_NimCasaChiPhiVon(lFinancial, code);
            var streamDoanhThu = await Chart_DoanhThu_LoiNhuan(lFinancial.Select(x => new BaseFinancialDTO { d = x.d, rv = x.rv, pf = x.pf }).ToList(), code);
            lOutput.Add(streamTangTruongTinDung);
            lOutput.Add(streamNoXau);
            lOutput.Add(streamNim);
            lOutput.Add(streamDoanhThu);
            return lOutput;
        }
        private async Task<Stream> Chart_NganHang_TangTruongTinDung(List<Financial> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);

                var basicColumn = new HighchartTangTruongTinDung($"{code} - Tăng trưởng tín dụng Quý {time.Item3}/{time.Item2} (YoY)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), new List<HighChartSeries_TangTruongTinDung>
                {
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Room tín dụng",
                        type = "column",
                        data = lTake.Select(x => x.room ?? 0).ToList(),
                        color = "rgba(158, 159, 163, 0.5)",
                        pointPlacement = -0.2,
                        dataLabels = new HighChartDataLabel()
                    },
                    new HighChartSeries_TangTruongTinDung
                    {
                        name="Tăng trưởng tín dụng",
                        type = "column",
                        data = lTake.Select(x => x.credit_r ?? 0).ToList(),
                        color = "#012060",
                        dataLabels = new HighChartDataLabel()
                    }
                });
                var strTitleYAxis = "(Đơn vị: %)";
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strTitleYAxis }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_TangTruongTinDung|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        private async Task<Stream> Chart_NganHang_NoXau(List<Financial> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();

                var yearPrev = time.Item2;
                var quarterPrev = time.Item3;
                if (time.Item3 > 1)
                {
                    quarterPrev--;
                }
                else
                {
                    quarterPrev = 4;
                    yearPrev--;
                }

                var lBaoPhu = new List<double>();
                var lTiLeNo = new List<double>();
                var lTangTruongNoNhom2 = new List<double>();
                var lTangTruongTrichLap = new List<double>();
                foreach (var item in lFinancial)
                {
                    double baophu = 0, tileNoxau = 0, tangTruongTrichLap = 0, tangTruongNoNhom2 = 0;
                    var noxau = item.debt3 + item.debt4 + item.debt5;
                    if (noxau > 0)
                    {
                        baophu = Math.Round(100 * (item.risk ?? 0) / noxau, 1);
                    }
                    //if (item.debt > 0)
                    //{
                    //    tileNoxau = Math.Round((float)(100 * noxau) / item.debt, 1);
                    //}

                    var prev = lFinancial.FirstOrDefault(x => x.d == item.d.GetPrevQuarter());
                    if (prev != null)
                    {
                        //if (prev.risk > 0)
                        //{
                        //    tangTruongTrichLap = Math.Round(100 * (-1 + (item.risk ?? 0) / (prev.risk ?? 1)), 1);
                        //}
                        if (prev.debt2 > 0)
                        {
                            tangTruongNoNhom2 = Math.Round(100 * (-1 + (float)item.debt2 / prev.debt2), 1);
                        }
                    }

                    //tang truong tin dung, room tin dung
                    lBaoPhu.Add(baophu);
                    //lTiLeNo.Add(tileNoxau);
                    lTangTruongNoNhom2.Add(tangTruongNoNhom2);
                    //lTangTruongTrichLap.Add(tangTruongTrichLap);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                     new() {
                        data = lBaoPhu.TakeLast(StaticVal._TAKE),
                        name = "Bao phủ nợ xấu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#012060"
                    },
                    //new()
                    //{
                    //    data = lTiLeNo.TakeLast(StaticVal._TAKE),
                    //    name = "Tỉ lệ nợ xấu",
                    //    type = "spline",
                    //    dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                    //    color = "#C00000",
                    //    yAxis = 1,
                    //},
                    new()
                    {
                        data = lTangTruongNoNhom2.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng nợ nhóm 2",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1
                    },
                    //new()
                    //{
                    //    data = lTangTruongTrichLap.TakeLast(StaticVal._TAKE),
                    //    name = "Tăng trưởng trích lập",
                    //    type = "spline",
                    //    dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                    //    color = "rgba(158, 159, 163, 0.5)",
                    //    yAxis = 1
                    //}
                };
                return await Chart_BasicBase($"{code} - Nợ xấu Quý {time.Item3}/{time.Item2} (QoQoY)", lFinancial.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameQuarter()).ToList(), lSeries, "(Bao phủ nợ xấu: %)", "(Tăng trưởng: %)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_NoXau|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        private async Task<Stream> Chart_NganHang_NimCasaChiPhiVon(List<Financial> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();

                var lNim = new List<double>();
                var lCasa = new List<double>();
                var lCir = new List<double>();
                var lChiPhiVon = new List<double>();
                foreach (var item in lFinancial)
                {
                    //tang truong tin dung, room tin dung
                    lNim.Add(item.nim_r ?? 0);
                    lCasa.Add(item.casa_r ?? 0);
                    lCir.Add(item.cir_r ?? 0);
                    lChiPhiVon.Add(item.cost_r ?? 0);
                }
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lNim.TakeLast(StaticVal._TAKE),
                        name = "NIM",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lCasa.TakeLast(StaticVal._TAKE),
                        name = "CASA",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lCir.TakeLast(StaticVal._TAKE),
                        name = "CIR",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lChiPhiVon.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng chi phí vốn",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "rgba(158, 159, 163, 0.5)",
                        yAxis = 1
                    }
                };
                return await Chart_BasicBase($"{code} - NIM, CASA, CIR, Chi phí vốn Quý {time.Item3}/{time.Item2}", lFinancial.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameQuarter()).ToList(), lSeries, "(NIM: %)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NganHang_NimCasaChiPhiVon|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
