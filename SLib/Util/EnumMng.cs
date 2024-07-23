using System.ComponentModel.DataAnnotations;


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
        GDNN_today = 10,
        GDNN_week = 11,
        GDNN_month = 12,
        ThongKeNhomNganh_today = 20,
        ThongKeNhomNganh_week = 21,
        ThongKeNhomNganh_month = 22,
        ChiBaoKyThuat = 30,
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
        Chart_VonHoa = 20,//Vốn hóa
        Other = 10
    }

    public enum E24HGDNNMode
    {
        [Display(Name = "10")]
        HSX = 1,
        [Display(Name = "02")]
        HNX = 2,
        [Display(Name = "03")]
        UPCOM = 3
    }

    public enum E24hGDNNType
    {
        [Display(Name = "today")]
        today = 1,
        [Display(Name = "week")]
        week = 2,
        [Display(Name = "month")]
        month = 3
    }

    public enum EHotkey
    {
        vonhoa = 1,
        nh_nim = 2,
        nh_casa =3,
        nh_noxau = 4,
        nh_trichlapduphong = 5,
        nh_tangtruongtindung= 6,
    }

    public enum ESheetCol
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6,
        G = 7,
        H = 8,
        I = 9,
        J = 10,
        K = 11,
        L = 12,
        M = 13,
        N = 14,
        O = 15,
        P = 16,
        Q = 17,
        R = 18,
        S = 19,
        T = 20,
        U = 21,
        V = 22,
        W = 23,
        X = 24,
        Y = 25,
        Z = 26,
        AA = 27,
        AB = 28,
        AC = 29,
        AD = 30,
        AE = 31,
        AF = 32,
        AG = 33,
        AH = 34
    }
}
