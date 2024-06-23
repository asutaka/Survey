﻿using System.ComponentModel.DataAnnotations;


namespace StockLibrary.Util
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
    public enum ETransactionType
    {
        [Display(Name = "Tự doanh")]
        TuDoanh = 1,
        [Display(Name = "Nước ngoài")]
        NuocNgoai = 2
    }
}
