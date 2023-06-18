using AutoMapper;
using BE_TKDecor.Core.Dtos.Category;
using BE_TKDecor.Core.Dtos.Product;
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
                .ForMember(dev => dev.RoleName, 
                src => src.MapFrom(x => x.Role.Name));

            CreateMap<UserRegisterDto, User>();

            // category
            CreateMap<Category, CategoryGetDto>();

            // product
            CreateMap<Product, ProductGetDto>()
                .ForMember(dev => dev.CategoryName,
                src => src.MapFrom(x => x.Category.Name));
        }
    }
}
