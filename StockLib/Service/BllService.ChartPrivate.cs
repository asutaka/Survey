﻿using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public partial class BllService
    {
        //Item1: s; Item2: rv; Item3: pf
        private async Task<Stream> Chart_DoanhThuBase(List<string> lMaChungKhoan,
                                                                int d,
                                                                IEnumerable<(string, double, double, double, double)> lCur,
                                                                IEnumerable<(string, double, double, double, double)> lPrev,
                                                                IEnumerable<(string, double)> lEx = null,
                                                                bool isTangTruongDoanhThu = false,
                                                                bool isTangTruongLoiNhuan = false,
                                                                bool isBienLoiNhuan = false,
                                                                bool isBienLoiNhuanGop = false,
                                                                bool isBienLoiNhuanRong = false)
        {
            try
            {
                var time = GetCurrentTime();
                var lDoanhThu = new List<double>();
                var lLoiNhuan = new List<double>();
                var lTangTruongDoanhThu = new List<double>();
                var lTangTruongLoiNhuan = new List<double>();
                var lBienLoiNhuan = new List<double>();
                var lBienLoiNhuanGop = new List<double>();
                var lBienLoiNhuanRong = new List<double>();
                foreach (var item in lMaChungKhoan)
                {
                    var cur = lCur.FirstOrDefault(x => x.Item1 == item);
                    if (cur.Item1 is null)
                    {
                        lDoanhThu.Add(0);
                        lLoiNhuan.Add(0);
                        lTangTruongDoanhThu.Add(0);
                        lTangTruongLoiNhuan.Add(0);
                        lBienLoiNhuan.Add(0);
                        continue;
                    }
                    var prev = lPrev.FirstOrDefault(x => x.Item1 == item);

                    //tang truong doanh thu, tang truong loi nhuan
                    lDoanhThu.Add(cur.Item2);
                    lLoiNhuan.Add(cur.Item3);

                    if(isTangTruongDoanhThu)
                    {
                        if (prev.Item1 is null)
                        {
                            lTangTruongDoanhThu.Add(0);
                        }
                        else
                        {
                            var rateRevenue = (cur.Item2 / (prev.Item2 == 0 ? 0.9 : prev.Item2));
                            if (rateRevenue > 100)
                                rateRevenue = 100;

                            lTangTruongDoanhThu.Add(Math.Round((-1 + rateRevenue) * 100, 1));
                        }
                    }

                    if (isTangTruongLoiNhuan)
                    {
                        if (prev.Item1 is null)
                        {
                            lTangTruongLoiNhuan.Add(0);
                        }
                        else
                        {
                            var rateProfit = (cur.Item3 / (prev.Item3 == 0 ? 0.9 : prev.Item3));
                            if (rateProfit > 100)
                                rateProfit = 100;

                            lTangTruongLoiNhuan.Add(Math.Round((-1 + rateProfit) * 100, 1));
                        }
                    }
                   
                    if(isBienLoiNhuan)
                    {
                        //Biên Lợi Nhuận
                        lBienLoiNhuan.Add(cur.Item2 == 0 ? 0 : Math.Round(cur.Item3 * 100 / cur.Item2, 1));
                    }

                    if (isBienLoiNhuanGop)
                    {
                        //Biên Lợi Nhuận Gộp
                        lBienLoiNhuanGop.Add(cur.Item2 == 0 ? 0 : Math.Round(cur.Item4 * 100 / cur.Item2, 1));
                    }

                    if (isBienLoiNhuanRong)
                    {
                        //Biên Lợi Nhuận Ròng
                        lBienLoiNhuanRong.Add(cur.Item2 == 0 ? 0 : Math.Round(cur.Item5 * 100 / cur.Item2, 1));
                    }
                }
                var basicColumn = new HighchartBasicColumn($"Doanh Thu, Lợi Nhuận Quý {time.Item3}/{time.Item2} (QoQoY)", lMaChungKhoan.ToList(), new List<HighChartSeries_BasicColumn>
                {
                     new HighChartSeries_BasicColumn
                    {
                        data = lDoanhThu,
                        name = "Doanh thu",
                        type = "column",
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lLoiNhuan,
                        name = "Lợi nhuận",
                        type = "column",
                        color = "#C00000"
                    }
                });
                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = "(Đơn vị: tỷ)" }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = string.Empty }, labels = new HighChartLabel{ format = "{value} %" }, opposite = true }};

                if (isTangTruongDoanhThu)
                {
                    //Tăng trưởng Doanh Thu
                    basicColumn.series.Add(new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongDoanhThu,
                        name = "Tăng trưởng DT",
                        type = "spline",
                        color = "#012060",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    });
                }

                if (isTangTruongLoiNhuan)
                {
                    //Tăng trưởng Lợi Nhuận
                    basicColumn.series.Add(new HighChartSeries_BasicColumn
                    {
                        data = lTangTruongLoiNhuan,
                        name = "Tăng trưởng LN",
                        type = "spline",
                        color = "#C00000",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    });
                }

                if (isBienLoiNhuan)
                {
                    //Biên Lợi Nhuận
                    basicColumn.series.Add(new HighChartSeries_BasicColumn
                    {
                        data = lBienLoiNhuan,
                        name = "Biên Lợi Nhuận",
                        type = "spline",
                        color = "#ffbf00",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    });
                }

                if (isBienLoiNhuanGop)
                {
                    //Biên Lợi Nhuận Gộp
                    basicColumn.series.Add(new HighChartSeries_BasicColumn
                    {
                        data = lBienLoiNhuanGop,
                        name = "Biên Lợi Nhuận Gộp",
                        type = "spline",
                        color = "#ffbf00",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    });
                }

                if (isBienLoiNhuanRong)
                {
                    //Biên Lợi Nhuận Ròng
                    basicColumn.series.Add(new HighChartSeries_BasicColumn
                    {
                        data = lBienLoiNhuanRong,
                        name = "Biên Lợi Nhuận Ròng",
                        type = "spline",
                        color = "#ffbf00",
                        dataLabels = new HighChartDataLabel(),
                        yAxis = 1
                    });
                }
                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_DoanhThu_LoiNhuanBase|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        public async Task<Stream> Chart_DoanhThu_LoiNhuan(List<BaseFinancialDTO> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();
                var lTangTruong = new List<double>();
                foreach (var item in lFinancial)
                {
                    double tangTruong = 0;
                    var prevQuarter = item.d.GetYoyQuarter();
                    var prev = lFinancial.FirstOrDefault(x => x.d == prevQuarter);
                    if (prev is not null && prev.pf != 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + item.pf / prev.pf), 1);
                        if (item.pf > prev.pf)
                        {
                            tangTruong = Math.Abs(tangTruong);
                        }
                        if (tangTruong >= StaticVal._MaxRate)
                        {
                            tangTruong = StaticVal._MaxRate;
                        }
                        if (tangTruong <= -StaticVal._MaxRate)
                        {
                            tangTruong = -StaticVal._MaxRate;
                        }
                    }

                    lTangTruong.Add(tangTruong);
                }
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.rv),
                        name = "Doanh thu",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                     new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.pf),
                        name = "Lợi nhuận",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng lợi nhuận",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"{code} - Doanh thu, Lợi nhuận Quý {time.Item3}/{time.Item2} (QoQ)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BDS_DoanhThu_LoiNhuan|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_NoTaiChinh(List<Financial> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();
                //var lTangTruong = new List<double>();
                //foreach (var item in lFinancial)
                //{
                //    double tangTruong = 0;
                //    var prevQuarter = item.d.GetPrevQuarter();
                //    var prev = lFinancial.FirstOrDefault(x => x.d == prevQuarter);
                //    if (prev is not null && prev.debt > 0)
                //    {
                //        tangTruong = Math.Round(100 * (-1 + item.debt / prev.debt), 1);
                //        if (tangTruong > 999)
                //            tangTruong = 999;
                //    }

                //    lTangTruong.Add(tangTruong);
                //}
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);
                var lVonChu = lTake.Select(x => Math.Round(x.debt * 100 / x.eq, 1)).ToList();
                if (lVonChu.Last() < 40)
                    return null;

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.debt),
                        name = "Nợ tài chính",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    //new HighChartSeries_BasicColumn
                    //{
                    //    data = lTangTruong.TakeLast(StaticVal._TAKE),
                    //    name = "Tăng trưởng nợ",
                    //    type = "spline",
                    //    dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                    //    color = "#C00000",
                    //    yAxis = 1,
                    //},
                    new HighChartSeries_BasicColumn
                    {
                        data = lVonChu,
                        name = "Nợ/ Vốn chủ",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#ffbf00",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"{code} - Nợ Quý {time.Item3}/{time.Item2} (QoQoY)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NoTaiChinh|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_TonKho(List<Financial> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();
                var lTangTruong = new List<double>();
                foreach (var item in lFinancial)
                {
                    double tangTruong = 0;
                    var prevQuarter = item.d.GetPrevQuarter();
                    var prev = lFinancial.FirstOrDefault(x => x.d == prevQuarter);
                    if (prev is not null && prev.inv > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + item.inv / prev.inv), 1);
                    }

                    lTangTruong.Add(tangTruong);
                }
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.inv),
                        name = "Tồn kho",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng tồn kho",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"{code} - Tồn kho Quý {time.Item3}/{time.Item2} (QoQoY)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_TonKho|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_FDI()
        {
            try
            {
                var lFDI = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.FDI));
                var maxD = lFDI.MaxBy(x => x.d).d;
                lFDI = lFDI.Where(x => x.d == maxD).OrderByDescending(x => x.va).Take(StaticVal._TAKE).ToList();

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lFDI.Select(x => Math.Round(x.va/1000, 1)),
                        name = "Vốn đăng ký",
                        type = "column",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    }
                };

                return await Chart_BasicBase($"Vốn FDI đăng ký", lFDI.Select(x => x.content).ToList(), lSeries, "giá trị: tỷ USD", "giá trị: %");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_FDI|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        private async Task<Stream> Chart_NguoiMua(List<Financial> lFinancial, string code)
        {
            try
            {
                var time = GetCurrentTime();
                var lTangTruong = new List<double>();
                foreach (var item in lFinancial)
                {
                    double tangTruong = 0;
                    var prevQuarter = item.d.GetPrevQuarter();
                    var prev = lFinancial.FirstOrDefault(x => x.d == prevQuarter);
                    if (prev is not null && prev.bp > 0)
                    {
                        tangTruong = Math.Round(100 * (-1 + item.bp / prev.bp), 1);
                    }

                    lTangTruong.Add(tangTruong);
                }
                var lTake = lFinancial.TakeLast(StaticVal._TAKE);

                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lTake.Select(x => x.bp),
                        name = "Người mua trả tiền trước",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lTangTruong.TakeLast(StaticVal._TAKE),
                        name = "Tăng trưởng người mua",
                        type = "spline",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}%" },
                        color = "#C00000",
                        yAxis = 1,
                    }
                };

                return await Chart_BasicBase($"{code} - Người mua trả tiền trước Quý {time.Item3}/{time.Item2} (QoQoY)", lTake.Select(x => x.d.GetNameQuarter()).ToList(), lSeries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_NguoiMua|EXCEPTION| {ex.Message}");
            }
            return null;
        }

        private async Task<Stream> Chart_XNK(IEnumerable<(double, double, string)> lVal, bool isXK, string title, string unit1, string unit2)
        {
            try
            {
                var strMode = isXK ? "xuất khẩu" : "nhập khẩu";
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lVal.TakeLast(25).Select(x => x.Item1),
                        name = $"Giá trị {strMode} {title}",
                        type = "column",
                        dataLabels = new HighChartDataLabel{ enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    }
                };

                if (lVal.Sum(x => x.Item2) > 0)
                {
                    lSeries.Add(new HighChartSeries_BasicColumn
                    {
                        data = lVal.TakeLast(25).Select(x => x.Item1),
                        name = $"Giá {title}",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000",
                        yAxis = 1
                    });
                }

                return await Chart_BasicBase($"{strMode} {title}", lVal.TakeLast(25).Select(x => x.Item3).ToList(), lSeries, $"giá trị: {unit1}", $"giá trị: {unit2}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_XNK|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        private async Task<Stream> Chart_ThongKe_BanLe()
        {
            try
            {
                var lBanLe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.BanLe)).OrderBy(x => x.d);
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lBanLe.TakeLast(StaticVal._TAKE).Select(x => Math.Round(x.va/1000, 1)),
                        name = "Tổng mức bán lẻ",
                        type = "column",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#012060"
                    },
                    new HighChartSeries_BasicColumn
                    {
                        data = lBanLe.TakeLast(StaticVal._TAKE).Select(x => x.qoq - 100),
                        name = "So với cùng kỳ",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000",
                        yAxis = 1
                    }
                };

                return await Chart_BasicBase($"Tổng mức bán lẻ so với cùng kỳ năm ngoái(QoQ)", lBanLe.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameMonth()).ToList(), lSeries, "giá trị: nghìn tỷ", "giá trị: %");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BanLe|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        private async Task<Stream> Chart_ThongKeQuy_CangBien()
        {
            try
            {
                var lThongKe = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.QUY_GiaVT_KhoBai)).OrderBy(x => x.d);
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lThongKe.TakeLast(StaticVal._TAKE).Select(x => x.qoq - 100),
                        name = "So với cùng kỳ",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000",
                        yAxis = 1
                    }
                };

                return await Chart_BasicBase($"Giá vận tải kho bãi quý so với cùng kỳ năm ngoái(QoQ)", lThongKe.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameQuarter()).ToList(), lSeries, "Đơn vị: %", "Đơn vị: %");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_ThongKeQuy_CangBien|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        private async Task<Stream> Chart_ThongKeQuy_HangKhong()
        {
            try
            {
                var lThongKe = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)EKeyTongCucThongKe.QUY_GiaVT_HangKhong)).OrderBy(x => x.d);
                var lSeries = new List<HighChartSeries_BasicColumn>
                {
                    new HighChartSeries_BasicColumn
                    {
                        data = lThongKe.TakeLast(StaticVal._TAKE).Select(x => x.qoq - 100),
                        name = "So với cùng kỳ",
                        type = "spline",
                        dataLabels = new HighChartDataLabel { enabled = true, format = "{point.y:.1f}" },
                        color = "#C00000",
                        yAxis = 1
                    }
                };

                return await Chart_BasicBase($"Giá vận tải hàng không quý so với cùng kỳ năm ngoái(QoQ)", lThongKe.TakeLast(StaticVal._TAKE).Select(x => x.d.GetNameQuarter()).ToList(), lSeries, "Đơn vị: %", "Đơn vị: %");
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_ThongKeQuy_HangKhong|EXCEPTION| {ex.Message}");
            }

            return null;
        }

        private async Task<Stream> Chart_XNK(Stock stock)
        {
            var eHaiquan = EHaiQuan.None;
            var eThongKe = EKeyTongCucThongKe.None;
            var strTitle = string.Empty;
            if (stock.cat.Any(x => x.ty == (int)EStockType.Thep))
            {
                eHaiquan = EHaiQuan.SatThep;
                eThongKe = EKeyTongCucThongKe.XK_SatThep;
                strTitle = "sắt thép";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.Than))
            {
                eHaiquan = EHaiQuan.Than;
                strTitle = "than";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.PhanBon))
            {
                eHaiquan = EHaiQuan.PhanBon;
                strTitle = "phân bón";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.HoaChat))
            {
                eHaiquan = EHaiQuan.HoaChat;
                eThongKe = EKeyTongCucThongKe.XK_HoaChat;
                strTitle = "hóa chất";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.Go))
            {
                eHaiquan = EHaiQuan.Go;
                eThongKe = EKeyTongCucThongKe.XK_Go;
                strTitle = "gỗ";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.Gao))
            {
                eHaiquan = EHaiQuan.Gao;
                eThongKe = EKeyTongCucThongKe.XK_Gao;
                strTitle = "gạo";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.XiMang))
            {
                eHaiquan = EHaiQuan.Ximang;
                eThongKe = EKeyTongCucThongKe.XK_Ximang;
                strTitle = "xi măng";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.CaPhe))
            {
                eThongKe = EKeyTongCucThongKe.XK_CaPhe;
                strTitle = "cà phê";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.CaoSu))
            {
                eHaiquan = EHaiQuan.CaoSu;
                eThongKe = EKeyTongCucThongKe.XK_CaoSu;
                strTitle = "cao su";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.Oto))
            {
                eHaiquan = EHaiQuan.Oto9Cho_NK;
                strTitle = "ô tô con";
            }
            if (stock.cat.Any(x => x.ty == (int)EStockType.OtoTai))
            {
                eHaiquan = EHaiQuan.OtoVanTai_NK;
                strTitle = "ô tô tải";
            }

            var lHaiQuan = _haiquanRepo.GetByFilter(Builders<ThongKeHaiQuan>.Filter.Eq(x => x.key, (int)eHaiquan)).OrderBy(x => x.d);
            var lThongKe = _thongkeRepo.GetByFilter(Builders<ThongKe>.Filter.Eq(x => x.key, (int)eThongKe)).OrderBy(x => x.d);
            var lThongKeQuy = _thongkequyRepo.GetByFilter(Builders<ThongKeQuy>.Filter.Eq(x => x.key, (int)eThongKe)).OrderBy(x => x.d);

            var lastHaiQuan = lHaiQuan?.LastOrDefault() ?? new ThongKeHaiQuan();//2024081
            var lastThongKe = lThongKe?.LastOrDefault() ?? new ThongKe();//202301
            var lastThongKeQuy = lThongKeQuy?.LastOrDefault() ?? new ThongKeQuy();//20231

            var yearHaiQuan = lastHaiQuan.d / 1000;
            var yearThongKe = lastThongKe.d / 100;
            var yearThongKeQuy = lastThongKeQuy.d / 10;

            var monthHaiQuan = (lastHaiQuan.d - yearHaiQuan * 1000) / 10;
            var monthThongKe = lastThongKe.d - yearThongKe * 100;
            var monthThongKeQuy = (lastThongKeQuy.d - yearThongKeQuy * 10) * 3;

            var dayHaiQuan = lastHaiQuan.d % 10 == 1 ? 15 : 26;
            var dayThongKe = 27;
            var dayThongKeQuy = 28;

            var dHaiQuan = new DateTime(yearHaiQuan < 1 ? 1 : yearHaiQuan, monthHaiQuan < 1 ? 1 : monthHaiQuan, dayHaiQuan < 1 ? 1 : dayHaiQuan);
            var dThongKe = new DateTime(yearThongKe < 1 ? 1 : yearThongKe, monthThongKe < 1 ? 1 : monthThongKe, dayThongKe < 1 ? 1 : dayThongKe);
            var dThongKeQuy = new DateTime(yearThongKeQuy < 1 ? 1 : yearThongKeQuy, monthThongKeQuy < 1 ? 1 : monthThongKeQuy, dayThongKeQuy < 1 ? 1 : dayThongKeQuy);

            var isNK = eHaiquan.ToString().Contains("NK") || eThongKe.ToString().Contains("NK");
            var lData = new List<(double, double, string)>();

            if (dHaiQuan > dThongKe && dHaiQuan > dThongKeQuy)
            {
                lData = lHaiQuan.Select(x => (x.va, x.price, x.d.GetNameHaiQuan())).ToList();
            }
            else if (dThongKe > dHaiQuan && dThongKe > dThongKeQuy)
            {
                lData = lThongKe.Select(x => (x.va, x.price, x.d.GetNameMonth())).ToList();
            }
            else
            {
                lData = lThongKeQuy.Select(x => (x.va, x.price, x.d.GetNameQuarter())).ToList();
            }
            return await Chart_XNK(lData, !isNK, strTitle, string.Empty, string.Empty);
        }

        private async Task<Stream> Chart_BasicBase(string title, List<string> lCat, List<HighChartSeries_BasicColumn> lSerie, string titleX = null, string titleY = null)
        {
            try
            {
                var basicColumn = new HighchartBasicColumn(title, lCat, lSerie);
                var strX = string.IsNullOrWhiteSpace(titleX) ? "(Đơn vị: tỷ)" : titleX;
                var strY = string.IsNullOrWhiteSpace(titleY) ? "(Tỉ lệ: %)" : titleY;

                basicColumn.yAxis = new List<HighChartYAxis> { new HighChartYAxis { title = new HighChartTitle { text = strX }, labels = new HighChartLabel{ format = "{value}" } },
                                                                 new HighChartYAxis { title = new HighChartTitle { text = strY }, labels = new HighChartLabel{ format = "{value}" }, opposite = true }};

                var chart = new HighChartModel(JsonConvert.SerializeObject(basicColumn));
                var body = JsonConvert.SerializeObject(chart);
                return await _apiService.GetChartImage(body);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.Chart_BasicBase|EXCEPTION| {ex.Message}");
            }
            return null;
        }
    }
}
