﻿using System.ComponentModel.DataAnnotations;


namespace SLib.Util
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

    public enum EHnxExchange
    {
        NY = 1,
        UP = 2
    }

    public enum EConfigDataType
    {
        TuDoanhHNX = 1,
        TuDoanhUpcom = 2,
        TuDoanhHose = 3,
        GDNN = 4
    }

    public enum ETransactionType
    {
        [Display(Name = "Tự doanh")]
        TuDoanh = 1,
        [Display(Name = "Nước ngoài")]
        NuocNgoai = 2
    }

    public enum ETimeMode
    {
        Day = 1,
        Week = 2
    }

    public enum EMessageMode
    {
        OnlyStock = 1,
        Other = 10
    }

    public enum E24HGDNNMode
    {
        [Display(Name = "10")]
        HSX = 1,
        [Display(Name = "02")]
        HNX = 2,
        [Display(Name = "03")]
        UP = 3
    }
}
