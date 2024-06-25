namespace StockLibrary.DAL
{
    public static class MongoSetting
    {
        public static string ConnectionString { get { return "mongodb+srv://asutaka:friend1312@cluster0.gntjvks.mongodb.net"; } }
        public static string Database { get { return "stockdb"; } }

        public static string ConnectionStringForeign { get { return "mongodb://192.168.1.13:27017"; } }
        public static string DatabaseForeign { get { return "TelegramSurvey"; } }
    }
}
