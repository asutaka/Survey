using SurveyStock.Model;
using SurveyStock.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace SurveyStock.DAL
{
    public static class sqliteComDB 
    {
        private static SQLiteConnection _con;
        private static string connectionString = $"Data Source={Directory.GetCurrentDirectory()}//db//comdb.db;Version=3;datetimeformat=CurrentCulture";

        public static SQLiteConnection Connect()
        {
            try
            {
                if (_con == null)
                    _con = new SQLiteConnection(connectionString);
                if (_con.State == ConnectionState.Closed)
                    _con.Open();
                Console.WriteLine("Connection is Opened ! ");
                return _con;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection is not Open ! ");
                throw;
            }
        }

        public static List<CompanyModel> GetData_Company()
        {
            var lResult = new List<CompanyModel>();
            using (var ds = new DataSet())
            {
                var sql = $"Select * from Company";
                var cmd = new SQLiteCommand(sql, Connect());
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lResult.Add(new CompanyModel
                    {
                        id = rdr.GetInt32(0),
                        company_name = rdr.GetString(1),
                        stock_code = rdr.GetString(2),
                        cap = rdr.GetDecimal(3),
                        stock_exchange = rdr.GetInt32(4),
                        status = rdr.GetInt32(5)
                    });
                }
            }
            return lResult;
        }

        public static List<CategoryModel> GetData_Category()
        {
            var lResult = new List<CategoryModel>();
            using (var ds = new DataSet())
            {
                var sql = $"Select * from Category";
                var cmd = new SQLiteCommand(sql, Connect());
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lResult.Add(new CategoryModel
                    {
                        id = rdr.GetInt32(0),
                        category_name = rdr.GetString(1),
                    });
                }
            }
            return lResult;
        }

        public static List<WhaleModel> GetData_Whale()
        {
            var lResult = new List<WhaleModel>();
            using (var ds = new DataSet())
            {
                var sql = $"Select * from Whale";
                var cmd = new SQLiteCommand(sql, Connect());
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lResult.Add(new WhaleModel
                    {
                        id = rdr.GetInt32(0),
                        whale_name = rdr.GetString(1),
                    });
                }
            }
            return lResult;
        }

        public static List<CompanyCategoryModel> GetData_CompanyCategory()
        {
            var lResult = new List<CompanyCategoryModel>();
            using (var ds = new DataSet())
            {
                var sql = $"Select * from Com_Category";
                var cmd = new SQLiteCommand(sql, Connect());
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lResult.Add(new CompanyCategoryModel
                    {
                        id_com = rdr.GetInt32(0),
                        id_category = rdr.GetInt32(1),
                        cap_rate = rdr.GetDecimal(2)
                    });
                }
            }
            return lResult;
        }

        public static List<CompanyWhaleModel> GetData_CompanyWhale()
        {
            var lResult = new List<CompanyWhaleModel>();
            using (var ds = new DataSet())
            {
                var sql = $"Select * from Com_Whale";
                var cmd = new SQLiteCommand(sql, Connect());
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lResult.Add(new CompanyWhaleModel
                    {
                        id_com = rdr.GetInt32(0),
                        id_whale = rdr.GetInt32(1),
                    });
                }
            }
            return lResult;
        }

        public static void Insert_Company(CompanyModel param)
        {
            var sql = $"INSERT INTO Company " +
                $"(id, company_name, stock_code, cap, stock_exchange, status) " +
                $"VALUES({param.id}, '{param.company_name}', '{param.stock_code}', {param.cap}, {param.stock_exchange}, {param.status})";
            var cmd = new SQLiteCommand(sql, Connect());
            cmd.ExecuteNonQuery();
        }

        public static void Insert_Category(CategoryModel param)
        {
            var sql = $"INSERT INTO Category " +
                $"(id, category_name) " +
                $"VALUES({param.id}, '{param.category_name}')";
            var cmd = new SQLiteCommand(sql, Connect());
            cmd.ExecuteNonQuery();
        }

        public static void Insert_Whale(WhaleModel param)
        {
            var sql = $"INSERT INTO Whale " +
                $"(id, whale_name) " +
                $"VALUES({param.id}, '{param.whale_name}')";
            var cmd = new SQLiteCommand(sql, Connect());
            cmd.ExecuteNonQuery();
        }

        public static void Insert_CompanyCategory(CompanyCategoryModel param)
        {
            var sql = $"INSERT INTO Com_Category " +
                $"(id_com, id_category, cap_rate) " +
                $"VALUES({param.id_com}, {param.id_category}, {param.cap_rate})";
            var cmd = new SQLiteCommand(sql, Connect());
            cmd.ExecuteNonQuery();
        }

        public static void Insert_CompanyWhale(CompanyWhaleModel param)
        {
            var sql = $"INSERT INTO Com_Whale " +
                $"(id_com, id_whale) " +
                $"VALUES({param.id_com}, {param.id_whale})";
            var cmd = new SQLiteCommand(sql, Connect());
            cmd.ExecuteNonQuery();
        }
    }

    public static class sqliteDayDB
    {
        private static Dictionary<EStockExchange, SQLiteConnection> _dicConnection = new Dictionary<EStockExchange, SQLiteConnection>();
        private static List<CompanyModel> _lCom = sqliteComDB.GetData_Company();

        public static void Instance()
        {
            var conHose = sqlite.Connect(null, $"Data Source={Directory.GetCurrentDirectory()}//db//hsxdb.db;Version=3;datetimeformat=CurrentCulture");
            var conHNX = sqlite.Connect(null, $"Data Source={Directory.GetCurrentDirectory()}//db//hnxdb.db;Version=3;datetimeformat=CurrentCulture");
            var conUpcom = sqlite.Connect(null, $"Data Source={Directory.GetCurrentDirectory()}//db//upcomdb.db;Version=3;datetimeformat=CurrentCulture");
            _dicConnection.Add(EStockExchange.Hose, conHose);
            _dicConnection.Add(EStockExchange.HNX, conHNX);
            _dicConnection.Add(EStockExchange.Upcom, conUpcom);
        }

        public static void CreateTable(string code, int type)
        {
            var itemConn = _dicConnection.First(x => x.Key == (EStockExchange)type);
            sqlite.CreateTable(code, itemConn.Value);
        }

        public static bool CheckTableExists(string code, int type)
        {
            var itemConn = _dicConnection.First(x => x.Key == (EStockExchange)type);
            return sqlite.CheckTableExists(code, itemConn.Value);
        }

        public static void Insert(string code, DataModel dat, int type)
        {
            try
            {
                var itemConn = _dicConnection.First(x => x.Key == (EStockExchange)type);
                sqlite.Insert(code, dat, itemConn.Value);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static List<DataModel> GetData(string code)
        {
            var entityCom = _lCom.FirstOrDefault(x => x.stock_code.ToUpper().Equals(code.ToUpper()));
            var itemConn = _dicConnection.First(x => x.Key == (EStockExchange)entityCom.stock_exchange);
            return sqlite.GetData(code, itemConn.Value);
        }

        public static void DeleteByTime(string code, decimal time, int type)
        {
            var itemConn = _dicConnection.First(x => x.Key == (EStockExchange)type);
            sqlite.DeleteByTime(code, time, itemConn.Value);
        }
    }

    public static class sqlite
    {
        public static SQLiteConnection Connect(string connectionString)
        {
            SQLiteConnection con = null;
            try
            {
                if (con == null)
                    con = new SQLiteConnection(connectionString);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    Console.WriteLine("Connection is Opened ! ");
                }
                return con;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection is not Open ! ");
                throw;
            }
        }
        public static SQLiteConnection Connect(SQLiteConnection con, string connectionString)
        {
            try
            {
                if (con == null)
                    con = new SQLiteConnection(connectionString);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    Console.WriteLine("Connection is Opened ! ");
                }
                return con;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection is not Open ! ");
                throw;
            }
        }

        public static bool CheckTableExists(string code, SQLiteConnection con)
        {
            try
            {
                using (var ds = new DataSet())
                {
                    //var sql = $"SELECT name FROM sqlite_master WHERE type='table' AND name='AAA'";
                    var sql = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{code}'";
                    var cmd = new SQLiteCommand(sql, con);
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public static void CreateTable(string code, SQLiteConnection con)
        {
            var sql = $"CREATE TABLE {code.ToUpper()} (t real, o real, h real, l real, c real, v real)";
            var cmd = new SQLiteCommand(sql, con);
            cmd.ExecuteNonQuery();
        }

        public static void Insert(string code, DataModel dat, SQLiteConnection con)
        {
            var sql = $"INSERT INTO {code.ToUpper()} " +
                $"(t, o, h, l, c, v) " +
                $"VALUES({dat.t}, {dat.o}, {dat.h}, {dat.l}, {dat.c}, {dat.v})";
            var cmd = new SQLiteCommand(sql, con);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteByTime(string code, decimal time, SQLiteConnection con)
        {
            var sql = $"DELETE from {code.ToUpper()} WHERE t = {time}";
            var cmd = new SQLiteCommand(sql, con);
            cmd.ExecuteNonQuery();
        }

        public static List<DataModel> GetData(string code, SQLiteConnection con)
        {
            var lResult = new List<DataModel>();
            try
            {
                using (var ds = new DataSet())
                {
                    var sql = $"Select * from {code.ToUpper()}";
                    var cmd = new SQLiteCommand(sql, con);
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        lResult.Add(new DataModel
                        {
                            t = rdr.GetDecimal(0),
                            o = rdr.GetDecimal(1),
                            h = rdr.GetDecimal(2),
                            l = rdr.GetDecimal(3),
                            c = rdr.GetDecimal(4),
                            v = rdr.GetDecimal(5)
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DataModel>();
            }
            
            return lResult;
        }
    }
}
