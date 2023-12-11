using AutoMapper;
using Skender.Stock.Indicators;
using Survey.Models;
using System;

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
