using ArooshyStore.Domain.DomainModels;
using ArooshyStore.Models.ViewModels;
using AutoMapper;

namespace ArooshyStore.App_Start
{
    public static class AutoMapperConfig
    {
        public static IMapper Mapper { get; private set; }
        public static void Init()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserViewModel, UserDomainModel>().ReverseMap();
                //cfg.CreateMap<UserViewModel, UserDomainModel>()
                //    //OrderId is different so map them using For Member
                //    .ForMember(dest => dest.FullName, act => act.MapFrom(src => src.FirstName + " " + src.LastName)).ReverseMap();
            });

            Mapper = config.CreateMapper();
        }
    }
}