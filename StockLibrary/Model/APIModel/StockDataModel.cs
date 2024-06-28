using System.Collections.Generic;

namespace StockLibrary.Model.APIModel
{
    public class StockDataModel
    {
        public IEnumerable<decimal> t { get; set; }
        public IEnumerable<decimal> c { get; set; }
        public IEnumerable<decimal> o { get; set; }
        public IEnumerable<decimal> h { get; set; }
        public IEnumerable<decimal> l { get; set; }
        public IEnumerable<decimal> v { get; set; }
    }
}
