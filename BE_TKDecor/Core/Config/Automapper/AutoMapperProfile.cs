using AutoMapper;
using BE_TKDecor.Core.Dtos.User;
using BusinessObject;

namespace BE_TKDecor.Core.Config.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // user
            CreateMap<User, UserGetDto>()
                .ForMember(dev => dev.RoleName, src => src.MapFrom(x => x.Role.Name));

            CreateMap<UserRegisterDto, User>();
        }
    }
}
