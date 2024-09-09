
namespace StockLib.DAL.Entity
{
    public class KeHoach : BaseDTO
    {
        public string s { get; set; }
        public int d { get; set; }
        public double pf_plan { get; set; }//Lợi nhuận kế hoạch
        public double pf_real { get; set; }//Lợi nhuận thực
        public double pf_real_r { get; set; }//Lợi nhuận thực %
        public double pf_cum { get; set; }//Lợi nhuận Lũy kế
        public double pf_cum_r { get; set; }//Lợi nhuận lũy kế %
    }
}
