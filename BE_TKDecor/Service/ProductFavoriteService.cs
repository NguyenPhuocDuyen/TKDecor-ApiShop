using AutoMapper;
using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class ProductFavoriteService : IProductFavoriteService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public ProductFavoriteService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetFavoriteOfUser(Guid userId, int pageIndex, int pageSize)
        {
            var list = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.ProductReview)
                    //.Include(x => x.ProductReviews)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete && x.Quantity > 0)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            try
            {
                var listProductFavorite = new List<ProductGetDto>();
                foreach (var product in list)
                {
                    var productDto = _mapper.Map<ProductGetDto>(product);

                    // Check if the user has liked the product or not
                    productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == userId);

                    int totalReviews = 0;
                    int totalRating = 0;

                    foreach (var orderDetail in product.OrderDetails)
                    {
                        if (orderDetail.ProductReview is not null)
                        {
                            totalReviews++;
                            totalRating += orderDetail.ProductReview.Rate;
                        }
                    }

                    if (totalReviews > 0)
                    {
                        double averageRating = (double)totalRating / totalReviews;
                        productDto.AverageRate = averageRating;
                    }
                    productDto.CountRate = totalReviews;

                    listProductFavorite.Add(productDto);
                }
                listProductFavorite = listProductFavorite.Where(x => x.IsFavorite).ToList();


                PaginatedList<ProductGetDto> pagingFavorites = PaginatedList<ProductGetDto>.CreateAsync(
                    listProductFavorite, pageIndex, pageSize);

                var result = new
                {
                    favorites = pagingFavorites,
                    pagingFavorites.PageIndex,
                    pagingFavorites.TotalPages,
                    pagingFavorites.TotalItem
                };

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> SetFavorite(Guid userId, FavoriteSetDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product is null || product.IsDelete)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            // Get current information whether the user likes this product
            // if not then add
            // yes then delete
            var productFavoriteDb = await _context.ProductFavorites
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == product.ProductId);

            try
            {
                if (productFavoriteDb is null)
                {
                    ProductFavorite newProductFavorite = new()
                    {
                        ProductId = product.ProductId,
                        UserId = userId
                    };
                    _context.ProductFavorites.Add(newProductFavorite);
                }
                else
                {
                    productFavoriteDb.UpdatedAt = DateTime.Now;
                    productFavoriteDb.IsDelete = !productFavoriteDb.IsDelete;
                    _context.ProductFavorites.Update(productFavoriteDb);
                }
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
