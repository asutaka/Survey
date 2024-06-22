namespace SurveyStock.DAL.MongoDAL
{
    public static class MongoSetting
    {
        public static string ConnectionString { get { return "mongodb+srv://asutaka:friend1312@cluster0.gntjvks.mongodb.net"; } }
        //public static string ConnectionString { get { return "mongodb://Dev:29313b348dd33950e81fc15eabe4dcb@192.168.8.132:27017/Blockchain_AAI"; } }
        //public static string Database { get { return "Blockchain_AAI"; } }
        public static string Database { get { return "stockdb"; } }
    }
}
