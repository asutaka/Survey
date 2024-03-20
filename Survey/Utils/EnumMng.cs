using System.ComponentModel.DataAnnotations;

namespace Survey.Utils
{
    public enum EInterval
    {
        I15M = 1,
        I1H = 2,
        I4H = 3,
        I1D = 4,
        //I1W = 5,
        I1M = 6
    }

    public enum EProgress
    {
        [Display(Name = "")]
        Idle = 0,
        [Display(Name = "Lấy thông tin Coin")]
        Start = 1,
        [Display(Name = "Đang lấy dữ liệu")]
        GetData = 2,
        [Display(Name = "Phân tích dữ liệu")]
        Analyze = 3
    }
}
