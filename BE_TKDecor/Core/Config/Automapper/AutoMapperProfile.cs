using AutoMapper;
using BE_TKDecor.Core.Dtos.Article;
using BE_TKDecor.Core.Dtos.Cart;
using BE_TKDecor.Core.Dtos.Category;
using BE_TKDecor.Core.Dtos.Coupon;
using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.Order;
using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Dtos.Product3DModel;
using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Dtos.ProductReview;
using BE_TKDecor.Core.Dtos.ReportProductReview;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Dtos.UserAddress;
using BusinessObject;
using Utility.SD;

namespace BE_TKDecor.Core.Config.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // user
            CreateMap<UserRegisterDto, User>();
            CreateMap<User, UserGetDto>();

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
            CreateMap<Coupon, CouponGetDto>();

            // cart 
            CreateMap<CartCreateDto, Cart>();
            CreateMap<Cart, CartGetDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(x => x.Product.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(x => x.Product.Price))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(opt => opt.Product.ProductImages.Select(x => x.ImageUrl).ToList()));

            // order
            CreateMap<Order, OrderGetDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
            CreateMap<OrderDetail, OrderDetailGetDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.Product.ProductImages.Select(pi => pi.ImageUrl).ToList()));

            // product review
            CreateMap<ProductReview,  ProductReviewGetDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
                .ForMember(dest => dest.Like, opt => opt.MapFrom(src => src.ProductReviewInteractions.Where(x => x.Interaction == Interaction.Like).Count()))
                .ForMember(dest => dest.DisLike, opt => opt.MapFrom(src => src.ProductReviewInteractions.Where(x => x.Interaction == Interaction.DisLike).Count()));

            // product review interaction
            //CreateMap<ProductReviewInteraction, ProductReviewInteractionGetDto>();

            // product report
            CreateMap<ProductReport, ProductReportGetDto>()
                .ForMember(dest => dest.UserReportName, opt => opt.MapFrom(src => src.UserReport.FullName))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductReported.Name));

            // report product review
            CreateMap<ReportProductReview, ReportProductReviewGetDto>()
                .ForMember(dest => dest.UserReportName, opt => opt.MapFrom(src => src.UserReport.FullName))
                .ForMember(dest => dest.UserReportEmail, opt => opt.MapFrom(src => src.UserReport.Email))
                .ForMember(dest => dest.ProductReviewReportedDescription, opt => opt.MapFrom(src => src.ProductReviewReported.Description));

            // notification
            CreateMap<Notification, NotificationGetDto>();

            // product 3d model
            CreateMap<Product3DModelCreateDto, Product3DModel>();
            CreateMap<Product3DModel, Product3DModelGetDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        }
    }
}
