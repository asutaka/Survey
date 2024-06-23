using System.Collections.Generic;

namespace StockLibrary.Model.APIModel
{
    public class ForeignModel
    {
        public List<Foreign> data { get; set; }
        public ForeignPageModel paging { get; set; }
    }

    public class ForeignPageModel
    {
        public int total { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}

