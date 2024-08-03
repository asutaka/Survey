using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SLib.Util;

namespace SLib.Service
{
    public partial class BllService
    {
        public async Task BCTCRead(string path)
        {
            try
            {
                var stream = await _apiService.BCTCRead(path);
                if (stream is null)
                    return;
                var lText = await pdfTextList(stream);
                Analyze(lText);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.BCTCRead|EXCEPTION| {ex.Message}");
            }
        }

        public async Task BCTCImport(Stream input)
        {
            try
            {
                var lText = await pdfTextList(input);
                Analyze(lText);

                //var stream = await _apiService.BCTCRead(input);
                //if (stream is null)
                //    return;
                //var lText = pdfTextList(stream);
                //Analyze(lText);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.BCTCImport|EXCEPTION| {ex.Message}");
            }
        }

        private async Task<List<string>> pdfTextList(Stream data)
        {
            var reader = new PdfReader(data);
            var lResult = new List<string>();
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                lResult.Add(PdfTextExtractor.GetTextFromPage(reader, page));
            }
            reader.Close();
            return lResult;
        }

        private void Analyze(List<string> lInput)
        {
            var tmp = LayDoanhThu(lInput);
            var output = new AnalyzeOutputModel
            {
                rv = LayDoanhThu(lInput),
                //pf = LayLoiNhuan(lInput),
                //inv = LayTonKho(lInput),
                //bp = LayNguoiMuaTraTienTruoc(lInput),
                //eq = LayVonChuSoHu(lInput),
                //la = LayVayNo(lInput),
                //ce = LayGiaVon(lInput)
            };
        }

        private DoanhThuModel LayDoanhThu(List<string> lInput)
        {
            try
            {
                var output = new DoanhThuModel
                {
                    ld = new List<DoanhThuDetailModel>()
                };
                int indexMain = -1;
                foreach (var item in lInput)
                {
                    indexMain++;
                    var textVietnam = item.FormatVietnamese();
                    var lSplit = textVietnam.Split("\n");
                    var indexSplit = -1;
                    var modeDoanhThu = false;
                    foreach (var row in lSplit)
                    {
                        indexSplit++;
                        if(!modeDoanhThu)
                        {
                            foreach (var itemKey in KeyMap.lDoanhThu)
                            {
                                if (row.Contains(itemKey))
                                {
                                    var lSpace = row.Split(" ");
                                    foreach (var space in lSpace)
                                    {
                                        var isValid = long.TryParse(space.Replace(",", "").Replace(".", ""), out var val);
                                        if (isValid && Math.Abs(val) >= 1000)
                                        {
                                            output.va = val;
                                            break;
                                        }
                                    }
                                }
                            }
                            var indexDetail = -1;
                            foreach (var itemKey in KeyMap.lDoanhThuChiTiet)
                            {
                                indexDetail++;
                                if (row.Contains(itemKey))
                                {
                                    var lSpace = row.Split(" ");
                                    var isPass = false;
                                    foreach (var space in lSpace)
                                    {
                                        var isValid = long.TryParse(space.Replace(",", "").Replace(".", ""), out var val);
                                        if (isValid && Math.Abs(val) >= 1000)
                                        {
                                            output.va = val;
                                            modeDoanhThu = true;
                                            isPass = true;
                                        }
                                    }
                                    if (!isPass)
                                    {
                                        var prev = lSplit[indexSplit - 1];
                                        foreach (var itemPrev in prev.Split(" "))
                                        {
                                            var isValid = long.TryParse(itemPrev.Replace(",", "").Replace(".", ""), out var val);
                                            if (isValid && Math.Abs(val) >= 1000)
                                            {
                                                output.va = val;
                                                modeDoanhThu = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var isHeader = row.StartsWith("-");
                            if (isHeader) 
                            {
                                var isPass = false;
                                foreach (var itemPrev in row.Split(" "))
                                {
                                    var isValid = long.TryParse(itemPrev.Replace(",", "").Replace(".", ""), out var val);
                                    if (isValid && Math.Abs(val) >= 1000)
                                    {
                                        var rate = Math.Round(100 * val / output.va, 1);
                                        if(rate >= 20)
                                        {
                                            var indexValue = row.IndexOf(itemPrev);
                                            output.ld.Add(new DoanhThuDetailModel
                                            {
                                                name = row.Substring(0, indexValue).Replace("-","").Trim(),
                                                va = val,
                                                rate = rate
                                            });
                                            isPass = true;
                                        }
                                        break;
                                    }
                                }
                                if(!isPass)
                                {
                                    var prev = lSplit[indexSplit - 1];
                                    foreach (var itemPrev in prev.Split(" "))
                                    {
                                        var isValid = long.TryParse(itemPrev.Replace(",", "").Replace(".", ""), out var val);
                                        if (isValid && Math.Abs(val) >= 1000)
                                        {
                                            var rate = Math.Round(100 * val / output.va, 1);
                                            if (rate >= 20)
                                            {
                                                var indexValue = row.IndexOf(itemPrev);
                                                output.ld.Add(new DoanhThuDetailModel
                                                {
                                                    name = row.Replace("-", "").Trim(),
                                                    va = val,
                                                    rate = rate
                                                });
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            if (output.ld.Sum(x => x.rate) >= 80)
                                return output;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"BllService.LayDoanhThu|EXCEPTION| {ex.Message}");
            }
            return null;
        }
        //private decimal LayLoiNhuan(List<string> lInput)
        //{
        //}
        //private TonKhoModel LayTonKho(List<string> lInput)
        //{

        //}
        //private NguoiMuaTraTienTruocModel LayNguoiMuaTraTienTruoc(List<string> lInput)
        //{

        //}
        //private decimal LayVonChuSoHu(List<string> lInput)
        //{

        //}
        //private VayNoModel LayVayNo(List<string> lInput)
        //{

        //}
        //private GiaVonModel LayGiaVon(List<string> lInput)
        //{

        //}
    }

    public class AnalyzeOutputModel
    {
        public DoanhThuModel rv { get; set; }//Doanh Thu
        public decimal pf { get; set; }//LNST
        public TonKhoModel inv { get; set; }//Tồn kho
        public NguoiMuaTraTienTruocModel bp { get; set; }//Người mua trả tiền trước
        public decimal eq { get; set; }//Vốn chủ sở hữu
        public VayNoModel la { get; set; }//Vay nợ
        public GiaVonModel ce { get; set; }//Giá vốn
    }

    public class TonKhoModel
    {
        public decimal va { get; set; }//Giá trị
        public List<TonKhoDetailModel> ld { get; set; }//Detail
    }
    public class TonKhoDetailModel
    {
        public string name { get; set; }
        public decimal va { get; set; }//Giá trị
        public decimal rate { get; set; }//Tỉ lệ
    }

    public class NguoiMuaTraTienTruocModel
    {
        public decimal va { get; set; }//Giá trị
        public List<NguoiMuaTraTienTruocDetailModel> ld { get; set; }//Detail
    }
    public class NguoiMuaTraTienTruocDetailModel
    {
        public string name { get; set; }
        public decimal va { get; set; }//Giá trị
        public decimal rate { get; set; }
    }

    public class VayNoModel
    {
        public decimal va { get; set; }//Giá trị
        public List<VayNoDetailModel> ld { get; set; }//Detail
    }
    public class VayNoDetailModel
    {
        public string name { get; set; }
        public decimal va { get; set; }//Giá trị
        public decimal rate { get; set; }
    }

    public class DoanhThuModel
    {
        public decimal va { get; set; }//Giá trị
        public List<DoanhThuDetailModel> ld { get; set; }//Detail
    }
    public class DoanhThuDetailModel
    {
        public string name { get; set; }
        public decimal va { get; set; }//Giá trị
        public decimal rate { get; set; }
    }

    public class GiaVonModel
    {
        public decimal va { get; set; }//Giá trị
        public List<GiaVonDetailModel> ld { get; set; }//Detail
    }
    public class GiaVonDetailModel
    {
        public string name { get; set; }
        public decimal va { get; set; }//Giá trị
        public decimal rate { get; set; }
    }
}
