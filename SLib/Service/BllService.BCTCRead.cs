using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
            var res = ExtractData(lInput);
            //Đầu tư tài chính
            //Tồn kho
            //Tổng tài sản
            //Người mua trả tiền trước 
            //Nợ ngắn hạn
            //Nợ dài hạn
            //Vốn chủ sở hữu
            //Chi phí vốn
        }

        private (double, double, double, double, double, double, double, double) ExtractData(List<string> lInput)
        {
            double DauTuTaiChinh = 0, TonKho = 0, TongTaiSan = 0, NguoiMuaTraTienTruoc = 0, NoNganHan = 0, NoDaiHan =0, VonChuSoHuu = 0, ChiPhiVon = 0;
            int div = 1000000000;
            try
            {
                int indexMain = -1;
                foreach (var item in lInput)
                {
                    indexMain++;
                    var textVietnam = item.FormatVietnamese();
                    var lSplit = textVietnam.Split("\n");
                    foreach (var row in lSplit)
                    {
                        Extract(row, KeyMap.lDauTuTaiChinh, ref DauTuTaiChinh);
                        Extract(row, KeyMap.lTonKho, ref TonKho);
                        Extract(row, KeyMap.lTongTaiSan, ref TongTaiSan);
                        Extract(row, KeyMap.lNguoiMuaTraTienTruoc, ref NguoiMuaTraTienTruoc);
                        Extract(row, KeyMap.lNoNganHan, ref NoNganHan);
                        Extract(row, KeyMap.lNoDaiHan, ref NoDaiHan);
                        Extract(row, KeyMap.lVonChuSoHuu, ref VonChuSoHuu);
                        Extract(row, KeyMap.lChiPhiVon, ref ChiPhiVon);
                    }
                }
                return (DauTuTaiChinh, TonKho, TongTaiSan, NguoiMuaTraTienTruoc, NoNganHan, NoDaiHan, VonChuSoHuu, ChiPhiVon);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BllService.LayDoanhThu|EXCEPTION| {ex.Message}");
            }
            return (0, 0, 0, 0, 0, 0, 0, 0);

            double Extract(string row, List<string> keyMap, ref double valCheck)
            {
                if (valCheck != 0)
                    return valCheck;
                foreach (var itemKey in keyMap)
                {
                    if (row.ToUpper().Contains(itemKey.ToUpper()))
                    {
                        var lSpace = row.Split(" ");
                        foreach (var space in lSpace)
                        {
                            var isValid = double.TryParse(space.Replace(",", "").Replace(".", ""), out var val);
                            if (isValid && Math.Abs(val) >= 1000)
                            {
                                valCheck = Math.Round(val / 1000000000, 1);
                                return valCheck;
                            }
                        }
                    }
                }
                return 0;
            }
        }
    }
}
