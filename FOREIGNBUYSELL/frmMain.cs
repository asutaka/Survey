using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UglyToad.PdfPig;

namespace FOREIGNBUYSELL
{
    public partial class frmMain : XtraForm
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnReadFile_Click(object sender, EventArgs e)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var lData = HSX(fileDialog.FileName);
                    foreach (var item in lData)
                    {
                        Console.WriteLine($"{item.STT} {item.MaCK} KLMua( {item.KLMua} ) KLBan( {item.KLBan} ) GiaTriMua( {item.GiaTriMua} ) GiaTriBan( {item.GiaTriBan} )");
                    }
                }
                catch
                {
                    MessageBox.Show("Lỗi không xác định", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public List<clsData> HSX(string path)
        {
            double posSTT = 0;
            double posMaCK = 0;

            double posMuaKL = 0;
            double posBanKL = 0;
            double posMuaGiaTri = 0;
            double posBanGiaTri = 0;
            double posMuaKL_ThoaThuan = 0;
            double posBanKL_ThoaThuan = 0;
            double posMuaGiaTri_ThoaThuan = 0;
            double tempVal = 0;

            using (PdfDocument document = PdfDocument.Open(path))
            {
                var checkNgay = false;
                var isComplete = false;
                int step = 0;

                var lData = new List<clsData>();
                var localData = new clsData();
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
                        if (!checkNgay 
                            && word.Contains("Ngày"))
                        {
                            if (isComplete)
                            {
                                lData.Add(localData);
                                return lData;
                            }
                            checkNgay = true;
                            continue;
                        }
                        if(checkNgay)
                        {
                            checkNgay = false;
                            isComplete = true;
                            posSTT = location.X;
                            posMaCK = letters.ElementAt(letters.Count - 1).Location.X;

                            var date = word.ToDateTime("dd/MM/yyyy");
                            if ((DateTime.Now - date).TotalDays >= 1)
                            {
                                var result = MessageBox.Show($"Thống kê không phải của ngày hiện tại( {word} ), tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (result == DialogResult.No)
                                    return lData;
                            }
                            continue;
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
                            else if(posBanKL <= 0)
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

                        if (location.X < posSTT)
                        {
                            var isSTT = int.TryParse(word, out var stt);
                            if (!isSTT)
                            {
                                continue;
                            }
                            //Console.WriteLine(word);
                            step = 1;

                            if(localData.STT > 0)
                            {
                                lData.Add(new clsData
                                {
                                    STT = localData.STT,
                                    MaCK = localData.MaCK,
                                    KLMua = localData.KLMua,
                                    KLBan = localData.KLBan,
                                    GiaTriMua = localData.GiaTriMua,
                                    GiaTriBan = localData.GiaTriBan
                                });
                                localData.MaCK = string.Empty;
                                localData.KLMua = 0;
                                localData.KLBan = 0;
                                localData.GiaTriMua = 0;
                                localData.GiaTriBan = 0;
                            }

                            localData.STT = stt;
                            continue;
                        }
                        else if (location.X >= posSTT
                            && location.X < posMaCK
                            && step == 1)
                        {
                            step = 2;
                            localData.MaCK = word;
                            //Console.WriteLine(word);
                        }
                        else if(location.X >= posMaCK
                            && location.X < posMuaKL
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if(isValid)
                                localData.KLMua = val;
                        }
                        else if (location.X >= posMuaKL
                            && location.X < posBanKL
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.KLBan = val;
                        }
                        else if (location.X >= posBanKL
                            && location.X < posMuaGiaTri
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.GiaTriMua = val;
                        }
                        else if (location.X >= posMuaGiaTri
                            && location.X < posBanGiaTri
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.GiaTriBan = val;
                        }
                        else if (location.X >= posBanGiaTri
                            && location.X < posMuaKL_ThoaThuan
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.KLMua += val;
                        }
                        else if (location.X >= posMuaKL_ThoaThuan
                           && location.X < posBanKL_ThoaThuan
                           && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.KLBan += val;
                        }
                        else if (location.X >= posBanKL_ThoaThuan
                          && location.X < posMuaGiaTri_ThoaThuan
                          && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.GiaTriMua += val;
                        }
                        else if(location.X >= posMuaGiaTri_ThoaThuan
                            && step == 2)
                        {
                            var isValid = int.TryParse(word, out var val);
                            if (isValid)
                                localData.GiaTriBan += val;
                        }
                    }
                }

                return lData;
            }
        }

        //public string HSX(string path)
        //{
        //    var posMuaKL = 0;
        //    var posBanKL = 0;
        //    var posMuaGiaTri = 0;
        //    var posBanGiaTri = 0;
        //    var posMuaKL_ThoaThuan = 0;
        //    var posBanKL_ThoaThuan = 0;
        //    var posMuaGiaTri_ThoaThuan = 0;

        //    using (PdfDocument document = PdfDocument.Open(path))
        //    {
        //        for (int i = 0; i < document.NumberOfPages; i++)
        //        {
        //            var page = document.GetPage(i + 1);
        //            var mcs = page.GetMarkedContents();

        //            foreach (var mc in mcs)
        //            {
        //                var letters = mc.Letters;
        //                var paths = mc.Paths;
        //                var images = mc.Images;
        //                //
        //                if (!letters.Any())
        //                    continue;
        //                var location = letters.ElementAt(0).Location;
        //                var word = string.Join("", letters.Select(x => x.Value));

        //                Console.WriteLine($"{word} {location}");


        //                //foreach (var letter in letters)
        //                //{
        //                //    if (letter.Value.Contains("ANV"))
        //                //    {
        //                //        var tmp = 1;
        //                //    }
        //                //    Console.Write(letter.Value);
        //                //    Console.Write(letter.Location);
        //                //}
        //                Console.WriteLine();
        //            }
        //        }
        //    }

        //    string text = string.Empty;
        //    return text;
        //}

        public string pdfText1(string path)
        {
            var reader = new iTextSharp.text.pdf.PdfReader(path);
            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                text += "\n";
            }
            reader.Close();
            return text;
        }

        //public void HSX(string path)
        //{
        //    var content = pdfText(fileDialog.FileName);
        //    var lIndexKeyword = content.AllIndexesOf("SỞ GIAO DỊCH CHỨNG KHOÁN TP. HỒ CHÍ MINH");
        //    if (!lIndexKeyword.Any())
        //        return;

        //    if(lIndexKeyword.Count >= 2)
        //    {
        //        content = content.Substring(lIndexKeyword.ElementAt(0), lIndexKeyword.ElementAt(1) - lIndexKeyword.ElementAt(0));
        //    }
        //    else
        //    {
        //        content = content.Substring(lIndexKeyword.ElementAt(0));
        //    }

        //    var indexNgay = content.IndexOf("Ngày");
        //    if (indexNgay < 0)
        //        return;
        //    content = content.Substring(indexNgay + 4);
        //    var lLine = content.Split(new string[] { "\n" }, StringSplitOptions.None);
        //    var dateStr = lLine.ElementAt(0).Trim();
        //    var date = dateStr.ToDateTime("dd/MM/yyyy");
        //    if ((DateTime.Now - date).TotalDays >= 1)
        //    {
        //        var result = MessageBox.Show($"Thống kê không phải của ngày hiện tại( {dateStr} ), tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //        if (result == DialogResult.No)
        //            return;
        //    }

        //    var isRead = false;
        //    var isBegin = false;
        //    var lData = new List<clsData>();
        //    var localModel = new clsData();
        //    foreach (var item in lLine)
        //    {
        //        try
        //        {
        //            if (item.Contains("Tổng cộng"))
        //            {
        //                isRead = true;
        //            }

        //            if (!isRead)
        //                continue;

        //            var arrData = item.Split(new string[] { " " }, StringSplitOptions.None);
        //            if (!isBegin
        //                && arrData.ElementAt(0) == "1")
        //            {
        //                isBegin = true;
        //            }
        //            if (!isBegin)
        //                continue;
        //            var count = arrData.Length;
        //            if (count >= 6)
        //            {
        //                var model = new clsData
        //                {
        //                    STT = int.Parse(arrData.ElementAt(0)),
        //                    MaCK = arrData.ElementAt(1),
        //                    KLMua = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", "")),
        //                    KLBan = int.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", "")),
        //                    GiaTriMua = int.Parse(arrData.ElementAt(4).Replace(",", "").Replace(".", "")),
        //                    GiaTriBan = int.Parse(arrData.ElementAt(5).Replace(",", "").Replace(".", "")),
        //                };
        //                lData.Add(model);
        //            }
        //            else
        //            {
        //                if (count == 2)
        //                {
        //                    localModel.STT = int.Parse(arrData.ElementAt(0));
        //                    localModel.MaCK = arrData.ElementAt(1);
        //                }
        //                else if (count == 4)
        //                {
        //                    var model = new clsData
        //                    {
        //                        STT = localModel.STT,
        //                        MaCK = localModel.MaCK,
        //                        KLMua = int.Parse(arrData.ElementAt(0).Replace(",", "").Replace(".", "")),
        //                        KLBan = int.Parse(arrData.ElementAt(1).Replace(",", "").Replace(".", "")),
        //                        GiaTriMua = int.Parse(arrData.ElementAt(2).Replace(",", "").Replace(".", "")),
        //                        GiaTriBan = int.Parse(arrData.ElementAt(3).Replace(",", "").Replace(".", "")),
        //                    };
        //                    lData.Add(model);
        //                }
        //                else
        //                {
        //                    localModel.STT = 0;
        //                    localModel.MaCK = string.Empty;
        //                }
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }

        //    var tmp1 = 1;
        //}
    }

    public class clsData
    {
        public int STT { get; set; }
        public string MaCK { get; set; }
        public int KLMua { get; set; }
        public int KLBan { get; set; }
        public int GiaTriMua { get; set; }
        public int GiaTriBan { get; set; }
    }
}