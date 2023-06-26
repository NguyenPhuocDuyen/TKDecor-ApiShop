using AutoMapper;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Dtos.Cart;
using BE_TKDecor.Core.Dtos.Category;
using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Dtos.ProductReview;
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
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(x => x.Role.Name));

            // category
            CreateMap<Category, CategoryGetDto>();
            CreateMap<CategoryCreateDto, Category>();

            // product
            CreateMap<ProductCreateDto, Product>();
            CreateMap<Product, ProductGetDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(x => x.Category.Name))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(opt => opt.ProductImages.Select(x => x.ImageUrl).ToList()))
                .ForMember(dest => dest.AverageRate, opt => opt.MapFrom(src => src.ProductReviews.Any() ? src.ProductReviews.Average(review => review.Rate) : 0))
                .ForMember(dest => dest.CountRate, opt => opt.MapFrom(src => src.ProductReviews.Count));

            // article 
            CreateMap<ArticleCreateDto, Article>();
            CreateMap<Article, ArticleGetDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.User.FullName));

            // product favorite 
            CreateMap<ProductFavorite, FavoriteGetDto>();

            // user address
            CreateMap<UserAddressCreateDto, UserAddress>();
            CreateMap<UserAddress, UserAddressGetDto>();

            // coupon 
            CreateMap<CouponCreateDto, Coupon>();
            CreateMap<Coupon, CouponGetDto>()
                .ForMember(dest => dest.CouponTypeName, opt => opt.MapFrom(x => x.CouponType.Name));

            // cart 
            CreateMap<CartCreateDto, Cart>();
            CreateMap<Cart, CartGetDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(x => x.Product.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(x => x.Product.Price))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(opt => opt.Product.ProductImages.Select(x => x.ImageUrl).ToList()));

            // order
            CreateMap<Order, OrderGetDto>();
            CreateMap<OrderDetail, OrderDetailGetDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.Product.ProductImages.Select(pi => pi.ImageUrl).ToList()));

            // product review
            //CreateMap<ProductReviewCreateDto,  ProductReview>();
            CreateMap<ProductReview,  ProductReviewGetDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl));
        }
    }
}
