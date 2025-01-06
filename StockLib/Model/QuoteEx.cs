using Skender.Stock.Indicators;

namespace StockLib.Model
{
    public class QuoteEx : Quote
    {
        public int Mode { get; set; }
        public decimal Entry { get; set; }
        public decimal SL { get; set; }
        public decimal Focus { get; set; }
    }
}
