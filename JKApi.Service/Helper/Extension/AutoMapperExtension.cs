using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.Helper.Extension
{
    public static class AutoMapperExtension
    {
        public static TDestination ToModel<TDestination, Source>(this object source)
        {
            Mapper.Initialize(one => one.AddProfile<MapProfile<TDestination, Source>>());
            return Mapper.Map<TDestination>(source);
        }
        public static IEnumerable<TView> MapEnumerable<TView, TDomainModel>(this IEnumerable<TDomainModel> domainEnumerable)
            where TDomainModel : class
            where TView : class
        {
            Mapper.Initialize(one => one.AddProfile<MapProfile<TDomainModel, TView>>());
            return AutoMapper.Mapper.Map<IEnumerable<TDomainModel>, IEnumerable<TView>>(domainEnumerable);
        }
    }
    internal class MapProfile<Destination, Source> : AutoMapper.Profile
    {
        public MapProfile()
        {
            CreateMap<Source, Destination>().ReverseMap();
        } 
    }
}
