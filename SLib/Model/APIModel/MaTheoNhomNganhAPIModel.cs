using System.Collections.Generic;

namespace SLib.Model.APIModel
{
    public class MaTheoNhomNganhAPIModel
    {
        public MaTheoNhomNganhAPIData data { get; set; }
        public int status { get; set; }
    }

    public class MaTheoNhomNganhAPIData
    {
        public List<MaTheoNhomNganhAPIDataDetail> data { get; set; }
    }
    public class MaTheoNhomNganhAPIDataDetail
    {
        public string symbol { get; set; }
    }
}
