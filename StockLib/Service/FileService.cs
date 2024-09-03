using Microsoft.Extensions.Logging;
using StockLib.DAL;
using StockLib.DAL.Entity;
using StockLib.Model;
using StockLib.Utils;

namespace StockLib.Service
{
    public interface IFileService
    {
        List<TudoanhPDF> HSX(Stream data);
        List<TudoanhPDF> HNX(Stream data);
        List<ThongKeHaiQuan> TongCucHaiQuan(Stream data, bool isXK);
    }
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IStockRepo _stockRepo;
        public FileService(ILogger<FileService> logger,
                        IStockRepo stockRepo)
        {
            _logger = logger;
            _stockRepo = stockRepo;
        }

        public List<TudoanhPDF> HSX(Stream data)
        {
            double posstt = 0;
            double poss = 0;

            double posMuaKL = 0;
            double posBanKL = 0;
            double posMuaGiaTri = 0;
            double posBanGiaTri = 0;
            double posMuaKL_ThoaThuan = 0;
            double posBanKL_ThoaThuan = 0;
            double posMuaGiaTri_ThoaThuan = 0;
            double tempVal = 0;

            using (var document = UglyToad.PdfPig.PdfDocument.Open(data))
            {
                var checkNgay = false;
                var setDate = false;
                int step = 0;

                var lData = new List<TudoanhPDF>();
                var localData = new TudoanhPDF();
                DateTime date = DateTime.MinValue;
                for (int i = 0; i < document.NumberOfPages; i++)
                {
                    var page = document.GetPage(i + 1);
                    var mcs = page.GetMarkedContents();

                    foreach (var mc in mcs)
                    {
                        var letters = mc.Letters;
                        var paths = mc.Paths;
                        var images = mc.Images;
                        //
                        if (!letters.Any())
                            continue;
                        var location = letters.ElementAt(0).Location;
                        var word = string.Join("", letters.Select(x => x.Value));
                        word = word.Replace(",", "").Replace(".", "");
                        if (!checkNgay)
                        {
                            if (!word.Contains("Ngày"))
                                continue;
                            checkNgay = true;
                            continue;
                        }

                        if (checkNgay && !setDate)
                        {
                            posstt = location.X;
                            poss = letters.ElementAt(letters.Count - 1).Location.X;

                            date = word.ToDateTime("dd/MM/yyyy");
                            setDate = true;
                            continue;
                        }

                        if (word.Contains("Ngày"))
                        {
                            lData.Add(localData);
                            return lData;
                        }

                        if (!setDate)
                            continue;

                        if (date == DateTime.MinValue)
                        {
                            return null;
                        }

                        if (posMuaGiaTri_ThoaThuan <= 0)
                        {
                            if (tempVal <= 0)
                            {
                                if (!word.Contains("Mua"))
                                    continue;
                                tempVal = location.X;
                                continue;
                            }

                            if (posMuaKL <= 0)
                            {
                                posMuaKL = (location.X + tempVal) / 2;
                            }
                            else if (posBanKL <= 0)
                            {
                                posBanKL = (location.X + tempVal) / 2;
                            }
                            else if (posMuaGiaTri <= 0)
                            {
                                posMuaGiaTri = (location.X + tempVal) / 2;
                            }
                            else if (posBanGiaTri <= 0)
                            {
                                posBanGiaTri = (location.X + tempVal) / 2;
                            }
                            else if (posMuaKL_ThoaThuan <= 0)
                            {
                                posMuaKL_ThoaThuan = (location.X + tempVal) / 2;
                            }
                            else if (posBanKL_ThoaThuan <= 0)
                            {
                                posBanKL_ThoaThuan = (location.X + tempVal) / 2;
                            }
                            else if (posMuaGiaTri_ThoaThuan <= 0)
                            {
                                posMuaGiaTri_ThoaThuan = (location.X + tempVal) / 2;
                            }

                            tempVal = location.X;
                            continue;
                        }

                        if (location.X < posstt)
                        {
                            var isstt = int.TryParse(word, out var stt);
                            if (!isstt)
                            {
                                continue;
                            }
                            step = 1;

                            if (localData.no > 0)
                            {
                                lData.Add(new TudoanhPDF
                                {
                                    no = localData.no,
                                    d = new DateTimeOffset(date, TimeSpan.FromHours(0)).ToUnixTimeSeconds(),
                                    s = localData.s,
                                    bvo = localData.bvo,
                                    svo = localData.svo,
                                    bva = localData.bva,
                                    sva = localData.sva,
                                    bvo_pt = localData.bvo_pt,
                                    svo_pt = localData.svo_pt,
                                    bva_pt = localData.bva_pt,
                                    sva_pt = localData.sva_pt,
                                    t = DateTimeOffset.Now.ToUnixTimeSeconds()
                                });
                                localData.s = string.Empty;
                                localData.bvo = 0;
                                localData.svo = 0;
                                localData.bva = 0;
                                localData.sva = 0;
                                localData.bvo_pt = 0;
                                localData.svo_pt = 0;
                                localData.bva_pt = 0;
                                localData.sva_pt = 0;
                            }

                            localData.no = stt;
                            continue;
                        }
                        else if (location.X >= posstt
                            && location.X < poss
                            && step == 1)
                        {
                            step = 2;
                            localData.s = word;
                        }
                        else if (location.X >= poss
                            && location.X < posMuaKL
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.bvo = val;
                        }
                        else if (location.X >= posMuaKL
                            && location.X < posBanKL
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.svo = val;
                        }
                        else if (location.X >= posBanKL
                            && location.X < posMuaGiaTri
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.bva = val;
                        }
                        else if (location.X >= posMuaGiaTri
                            && location.X < posBanGiaTri
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.sva = val;
                        }
                        else if (location.X >= posBanGiaTri
                            && location.X < posMuaKL_ThoaThuan
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.bvo_pt = val;
                        }
                        else if (location.X >= posMuaKL_ThoaThuan
                           && location.X < posBanKL_ThoaThuan
                           && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.svo_pt = val;
                        }
                        else if (location.X >= posBanKL_ThoaThuan
                          && location.X < posMuaGiaTri_ThoaThuan
                          && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.bva_pt = val;
                        }
                        else if (location.X >= posMuaGiaTri_ThoaThuan
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.sva_pt = val;
                        }
                    }
                }

                return lData;
            }
        }

        public List<TudoanhPDF> HNX(Stream data)
        {
            var content = pdfText(data);
            return MapHNX(content);
        }

        public List<ThongKeHaiQuan> TongCucHaiQuan(Stream data, bool isXK)
        {
            var content = pdfText(data);
            if (isXK)
                return MapTongCucHaiQuan_XuatKhau(content);
            else
                return MapTongCucHaiQuan_NhapKhau(content);
        }

        private string pdfText(Stream data)
        {
            var reader = new iTextSharp.text.pdf.PdfReader(data);

            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                text += "\n";
            }

            reader.Close();
            return text;
        }

        private List<ThongKeHaiQuan> MapTongCucHaiQuan_XuatKhau(string content)
        {
            var lLine = content.Split(new string[] { "\n" }, StringSplitOptions.None);
            var lData = new List<ThongKeHaiQuan>();
            bool isThuySan = false, isGao = false, isXiMang = false, isThan = false, isDauTho = false, isXangDau = false, 
                isHoaChat = false, isSPHoaChat = false, isPhanBon = false, isChatDeo = false, isSPChatDeo = false, isCaoSu = false, 
                isGo = false, isDetMay = false, isSatThep = false, isSPSatThep = false, isDayDien = false; 
            foreach (var item in lLine)
            {
                try
                {
                    var isTon = false;
                    var indexDVT = item.IndexOf("USD");
                    if(indexDVT < 0)
                    {
                        indexDVT = item.IndexOf("Tấn", StringComparison.OrdinalIgnoreCase);
                        isTon = true;
                    }
                    if (indexDVT < 0)
                        continue;

                    
                    var keyStr = item.Substring(0, indexDVT);
                    if(!isThuySan && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Thủy sản".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isThuySan = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.ThuySan));
                        continue;
                    }
                    if (!isGao && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Gạo".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isGao = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.Gao));
                        continue;
                    }
                    if (!isXiMang && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Xi măng".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isXiMang = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.Ximang));
                        continue;
                    }
                    if (!isThan && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Than".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isThan = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.Than));
                        continue;
                    }
                    if (!isDauTho && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Dầu thô".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isDauTho = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.DauTho));
                        continue;
                    }
                    if (!isXangDau && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Xăng dầu".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isXangDau = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.XangDau));
                        continue;
                    }
                    if (!isHoaChat && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Hóa chất".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isHoaChat = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.HoaChat));
                        continue;
                    }
                    if (!isSPHoaChat && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Hóa chất".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isSPHoaChat = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.SPHoaChat));
                        continue;
                    }
                    if (!isPhanBon && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Phân bón".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isPhanBon = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.PhanBon));
                        continue;
                    }
                    if (!isChatDeo && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Chất dẻo".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isChatDeo = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.ChatDeo));
                        continue;
                    }
                    if (!isSPChatDeo && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Chất dẻo".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isSPChatDeo = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.SPChatDeo));
                        continue;
                    }
                    if (!isCaoSu && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Cao su".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isCaoSu = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.CaoSu));
                        continue;
                    }
                    if (!isGo && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Gỗ".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isGo = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.Go));
                        continue;
                    }
                    if (!isDetMay && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Dệt may".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isDetMay = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.DetMay));
                        continue;
                    }
                    if (!isSatThep && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Sắt thép".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isSatThep = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.SatThep));
                        continue;
                    }
                    if (!isSPSatThep && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Sắt thép".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isSPSatThep = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.SPSatThep));
                        continue;
                    }
                    if (!isDayDien && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Dây điện".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isDayDien = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.DayDien));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            return lData;

            ThongKeHaiQuan DetectData(string item, int indexDVT, bool isTon, EHaiQuan e)
            {
                var model = new ThongKeHaiQuan
                {
                    key = (int)e
                };
                var arrData = item.Substring(indexDVT + 4).Split(new string[] { " " }, StringSplitOptions.None);
                if (isTon)
                {
                    var isDoubleWeight = double.TryParse(arrData[0].RemoveSpace(), out var valWeight);
                    var isDoubleVa = double.TryParse(arrData[1].RemoveSpace(), out var valVa);
                    if (isDoubleVa)
                    {
                        model.va = Math.Round(valVa / 1000000, 1);
                    }
                    if (isDoubleWeight && valWeight > 0)
                    {
                        model.price = Math.Round(valVa / valWeight, 1);
                    }
                }
                else
                {
                    var isDouble = double.TryParse(arrData[0].RemoveSpace(), out var val);
                    if (isDouble)
                    {
                        model.va = Math.Round(val / 1000000, 1);
                    }
                }
                return model;
            }
        }

        private List<ThongKeHaiQuan> MapTongCucHaiQuan_NhapKhau(string content)
        {
            var lLine = content.Split(new string[] { "\n" }, StringSplitOptions.None);
            var lData = new List<ThongKeHaiQuan>();
            bool isSPSatThep = false, isOtoDuoi9Cho = false, isOtoVanTai = false;
            foreach (var item in lLine)
            {
                try
                {
                    var isTon = false;
                    var indexDVT = item.IndexOf("USD");
                    if (indexDVT < 0)
                    {
                        indexDVT = item.IndexOf("Tấn", StringComparison.OrdinalIgnoreCase);
                        if(indexDVT < 0)
                        {
                            indexDVT = item.IndexOf("Chiếc", StringComparison.OrdinalIgnoreCase);
                        }
                        isTon = true;
                    }
                    if (indexDVT < 0)
                        continue;


                    var keyStr = item.Substring(0, indexDVT);
                    if (!isSPSatThep && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Sản phẩm từ Sắt thép".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isSPSatThep = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.SPSatThep_NK));
                        continue;
                    }
                    if (!isOtoDuoi9Cho && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Ô tô 9 chỗ".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isOtoDuoi9Cho = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.Oto9Cho_NK));
                        continue;
                    }
                    if (!isOtoVanTai && keyStr.RemoveSpace().RemoveSignVietnamese().Contains("Ô tô vận tải".RemoveSpace().RemoveSignVietnamese(), StringComparison.OrdinalIgnoreCase))
                    {
                        isOtoVanTai = true;
                        lData.Add(DetectData(item, indexDVT, isTon, EHaiQuan.OtoVanTai_NK));
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            return lData;

            ThongKeHaiQuan DetectData(string item, int indexDVT, bool isTon, EHaiQuan e)
            {
                var model = new ThongKeHaiQuan
                {
                    key = (int)e
                };
                var arrData = item.Substring(indexDVT).Split(new string[] { " " }, StringSplitOptions.None);
                if (isTon)
                {
                    var isDoubleWeight = double.TryParse(arrData[1].RemoveSpace(), out var valWeight);
                    var isDoubleVa = double.TryParse(arrData[2].RemoveSpace(), out var valVa);
                    if (isDoubleVa)
                    {
                        model.va = Math.Round(valVa / 1000000, 1);
                    }
                    if (isDoubleWeight && valWeight > 0)
                    {
                        model.price = Math.Round(valVa / valWeight, 1);
                    }
                }
                else
                {
                    var isDouble = double.TryParse(arrData[1].RemoveSpace(), out var val);
                    if (isDouble)
                    {
                        model.va = Math.Round(val / 1000000, 1);
                    }
                }
                return model;
            }
        }

        private List<TudoanhPDF> MapHNX(string content)
        {
            var indexFileHNX = content.IndexOf("Sở Giao dịch Chứng khoán Hà Nội");
            if (indexFileHNX < 0)
                return null;

            var strTieuDe = "GIAO DỊCH TỰ DOANH THEO MÃ CHỨNG KHOÁN";
            var indexNgay = content.IndexOf(strTieuDe);
            if (indexNgay < 0)
                return null;
            content = content.Substring(indexNgay);
            var lLine = content.Split(new string[] { "\n" }, StringSplitOptions.None);
            var dateStr = lLine.ElementAt(0).Replace(strTieuDe, "").Trim();
            var date = dateStr.ToDateTime("dd/MM/yyyy");

            var lStock = _stockRepo.GetAll();

            var isRead = false;
            var isBegin = false;
            var lData = new List<TudoanhPDF>();
            foreach (var item in lLine)
            {
                try
                {
                    if (item.Contains("Tổng GTGD Tự doanh"))
                    {
                        isRead = true;
                    }

                    if (!isRead)
                        continue;

                    var arrData = item.Split(new string[] { " " }, StringSplitOptions.None);
                    if (!isBegin
                        && arrData.ElementAt(0) == "1")
                    {
                        isBegin = true;
                    }
                    if (!isBegin)
                        continue;
                    var count = arrData.Length;
                    if (count < 8)
                        continue;

                    var maCKCheck = arrData.ElementAt(1).Trim();
                    var check = lStock.Any(x => x.s == maCKCheck);
                    if (check)
                    {
                        var model = new TudoanhPDF
                        {
                            no = int.Parse(arrData.ElementAt(0)),
                            d = new DateTimeOffset(date, TimeSpan.FromHours(0)).ToUnixTimeSeconds(),
                            s = arrData.ElementAt(1),
                            bvo = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", "")),
                            bva = (int)(long.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", "")) / 1000),
                            svo = int.Parse(arrData.ElementAt(4).Replace(",", "").Replace(".", "")),
                            sva = (int)(long.Parse(arrData.ElementAt(5).Replace(",", "").Replace(".", "")) / 1000),
                            t = DateTimeOffset.Now.ToUnixTimeSeconds()
                        };
                        lData.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            return lData;
        }
    }
}
