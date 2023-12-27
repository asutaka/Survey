using AutoMapper;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using Survey.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Survey.Utils
{
    public static class ExtensionMethod
    {
        public static Mapper MapperConfig = new Mapper(new MapperConfiguration(config => { config.AddProfile(new MapProfile()); }));
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static T To<T>(this FinancialDataPoint model)
        {
            if (model == null)
                return default(T);
            return MapperConfig.Map<T>(model);
        }

        public static T LoadJsonFile<T>(this int val, string fileName)
        {
            try
            {
                string path = $"{Directory.GetCurrentDirectory()}\\Config\\{fileName}.json";
                if (!File.Exists(path))
                    return default(T);
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    var result = JsonConvert.DeserializeObject<T>(json);
                    return result;
                }
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"ExtensionMethod.LoadJsonFile|EXCEPTION|INPUT: fileName: {fileName}| {ex.Message}");
                return default(T);
            }
        }

        public static bool UpdateJson<T>(this T _model, string fileName)
        {
            try
            {
                string path = $"{Directory.GetCurrentDirectory()}\\Config\\{fileName}.json";
                var check = File.Exists(path);
                if (!check)
                {
                    using (StreamWriter w = File.AppendText(path)) ;
                }
                string json = JsonConvert.SerializeObject(_model);
                //write string to file
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex, $"ExtensionMethod.UpdateJson|EXCEPTION|INPUT: fileName: {fileName}| {ex.Message}");
                return false;
            }
        }
        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<FinancialDataPoint, Quote>()
                //.ForMember(dest => dest.Open, opt => opt.MapFrom(src => (decimal)src.Open))
                //.ForMember(dest => dest.Close, opt => opt.MapFrom(src => (decimal)src.Close))
                //.ForMember(dest => dest.High, opt => opt.MapFrom(src => (decimal)src.High))
                //.ForMember(dest => dest.Low, opt => opt.MapFrom(src => (decimal)src.Low))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateTimeStamp));
        }
    }
}
