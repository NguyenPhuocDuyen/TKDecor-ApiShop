using AutoMapper;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Dtos.Category;
using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Dtos.UserAddress;
using BusinessObject;
using Microsoft.Build.Framework;

namespace BE_TKDecor.Core.Config.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // user
            CreateMap<UserRegisterDto, User>();
            CreateMap<User, UserGetDto>()
                .ForMember(dev => dev.RoleName, 
                src => src.MapFrom(x => x.Role.Name));

            // category
            CreateMap<Category, CategoryGetDto>();

            // product
            CreateMap<Product, ProductGetDto>()
                .ForMember(dev => dev.CategoryName,
                src => src.MapFrom(x => x.Category.Name));

            // article 
            CreateMap<ArticleCreateDto, Article>();
            CreateMap<Article, ArticleGetDto>()
                .ForMember(dev => dev.UserName,
                src => src.MapFrom(x => x.User.FullName));

            // product favorite 
            CreateMap<ProductFavorite, FavoriteGetDto>();

            // user address
            CreateMap<UserAddressCreateDto, UserAddress>();
            CreateMap<UserAddress, UserAddressGetDto>();
        }
    }
}
