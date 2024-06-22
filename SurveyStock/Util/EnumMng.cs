using System.ComponentModel.DataAnnotations;

namespace SurveyStock.Util
{
    public enum EStockExchange
    {
        [Display(Name = "hose")]
        Hose = 1,
        [Display(Name = "hnx")]
        HNX = 2,
        [Display(Name = "upcom")]
        Upcom = 3
    }

    public enum EModeBuySell
    {
        NoAction = 0,
        Listen = 1,
        Buy = 2,
    }

    public enum EmaType
    {
        MA = 1,
        EMA = 2
    }
}
