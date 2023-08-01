using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.Product;
using AutoMapper;
using BE_TKDecor.Core.Dtos.ProductReview;
using BusinessObject;
using Utility;
using System.Collections.Generic;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _product;
        private readonly IProductReviewRepository _productReview;
        private readonly IUserRepository _user;

        public ProductsController(IMapper mapper,
            IProductRepository product,
            IProductReviewRepository productReview,
            IUserRepository user)
        {
            _mapper = mapper;
            _product = product;
            _productReview = productReview;
            _user = user;
        }

        // GET: api/Products/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            Guid? categoryId,
            string search = "",
            string sort = "default",
            int pageIndex = 1,
            int pageSize = 20
            )
        {
            var list = await _product.GetAll();
            list = list.Where(x => !x.IsDelete && x.Quantity > 0)
                    .ToList();

            var user = await GetUser();

            // filter categoryId
            if (categoryId != null)
            {
                list = list.Where(x => x.CategoryId == categoryId).ToList();
            }

            // filter search
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Name.Contains(search)
                || x.Description.Contains(search)
                || x.Category.Name.Contains(search)
                ).ToList();
            }

            // map dto
            var listProductGet = new List<ProductGetDto>();
            foreach (var product in list)
            {
                var productDto = _mapper.Map<ProductGetDto>(product);

                // Check if the user has liked the product or not
                productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == user?.UserId);

                listProductGet.Add(productDto);
            }

            // filter sort
            listProductGet = sort switch
            {
                "price-high-to-low" => listProductGet.OrderByDescending(x => x.Price).ToList(),
                "price-low-to-high" => listProductGet.OrderBy(x => x.Price).ToList(),
                "average-rate" => listProductGet.OrderByDescending(x => x.AverageRate).ToList(),
                _ => listProductGet.OrderByDescending(x => x.CreatedAt).ToList(),
            };

            PaginatedList<ProductGetDto> pagingProduct = PaginatedList<ProductGetDto>.CreateAsync(
                listProductGet, pageIndex, pageSize);

            var result = new
            {
                products = pagingProduct,
                pagingProduct.PageIndex,
                pagingProduct.TotalPages,
                pagingProduct.TotalItem
            };

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products/FeaturedProducts
        [HttpGet("FeaturedProducts")]
        public async Task<IActionResult> FeaturedProducts()
        {
            var products = await _product.GetAll();
            products = products.Where(x => !x.IsDelete && x.Quantity > 0)
                    .OrderByDescending(x => x.OrderDetails.Sum(x => x.Quantity))
                    .Take(9)
                    .ToList();

            var user = await GetUser();
            var result = new List<ProductGetDto>();
            foreach (var product in products)
            {
                var productDto = _mapper.Map<ProductGetDto>(product);

                // Check if the user has liked the product or not
                productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == user?.UserId);

                result.Add(productDto);
            }

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products/GetReview/2
        [HttpGet("GetReview/{slug}")]
        public async Task<IActionResult> GetReview(
            string slug,
            string sort = "Default",
            int pageIndex = 1,
            int pageSize = 20
            )
        {
            var product = await _product.FindBySlug(slug);
            if (product == null || product.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var revews = await _productReview.FindByProductId(product.ProductId);
            revews = revews.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var user = await GetUser();
            var listReviewGetDto = new List<ProductReviewGetDto>();
            foreach (var review in revews)
            {
                var reviewDto = _mapper.Map<ProductReviewGetDto>(review);

                // Check if the user has liked the product or not
                var interaction = review.ProductReviewInteractions.FirstOrDefault(pf => !pf.IsDelete && pf.UserId == user?.UserId);
                if (interaction != null)
                {
                    reviewDto.InteractionOfUser = interaction.Interaction.ToString();
                }

                listReviewGetDto.Add(reviewDto);
            }

            // filter sort
            listReviewGetDto = sort switch
            {
                "rate-high-to-low" => listReviewGetDto.OrderByDescending(x => x.Rate).ToList(),
                "rate-low-to-high" => listReviewGetDto.OrderBy(x => x.Rate).ToList(),
                "rate-most-like" => listReviewGetDto.OrderByDescending(x => x.TotalLike).ToList(),
                _ => listReviewGetDto.OrderByDescending(x => x.CreatedAt).ToList(),
            };

            PaginatedList<ProductReviewGetDto> pagingReviews = PaginatedList<ProductReviewGetDto>.CreateAsync(
               listReviewGetDto, pageIndex, pageSize);

            var result = new
            {
                reviews = pagingReviews,
                pagingReviews.PageIndex,
                pagingReviews.TotalPages,
                pagingReviews.TotalItem
            };

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products/RelatedProducts/slug
        [HttpGet("RelatedProducts/{slug}")]
        public async Task<IActionResult> RelatedProducts(string slug)
        {
            var p = await _product.FindBySlug(slug);

            if (p == null || p.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var productList = await _product.GetAll();

            productList = productList.Where(x => x.ProductId != p.ProductId
                && x.CategoryId == p.CategoryId)
                .Take(5)
                .ToList();

            var user = await GetUser();
            // map dto
            var listProductGet = new List<ProductGetDto>();
            foreach (var product in productList)
            {
                var productDto = _mapper.Map<ProductGetDto>(product);

                // Check if the user has liked the product or not
                productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == user?.UserId);

                listProductGet.Add(productDto);
            }

            return Ok(new ApiResponse { Success = true, Data = listProductGet });
        }

        // GET: api/Products/GetBySlug/5
        [HttpGet("GetBySlug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var product = await _product.FindBySlug(slug);

            if (product == null || product.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var result = _mapper.Map<ProductGetDto>(product);
            var user = await GetUser();
            result.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == user?.UserId);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.FindById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
