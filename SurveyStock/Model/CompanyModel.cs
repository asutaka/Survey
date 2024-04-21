namespace SurveyStock.Model
{
    public class CompanyModel
    {
        public int id { get; set; }
        public string company_name { get; set; }
        public string stock_code { get; set; }
        public int stock_exchange { get; set; }
        public decimal cap { get; set; }
    }

    public class WhaleModel
    {
        public int id { get; set; }
        public string whale_name { get; set; }
    }

    public class CategoryModel
    {
        public int id { get; set; }
        public string category_name { get; set; }
    }

    public class CompanyCategoryModel
    {
        public int id_com { get; set; }
        public int id_category { get; set; }
        public decimal cap_rate { get; set; }
    }

    public class CompanyWhaleModel
    {
        public int id_com { get; set; }
        public int id_whale { get; set; }
    }
}
