using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.Product;
using AutoMapper;
using BE_TKDecor.Core.Dtos.ProductReview;
using BusinessObject;
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
        public async Task<IActionResult> GetAll()
        {
            var list = await _product.GetAll();
            list = list.Where(x => !x.IsDelete && x.Quantity > 0)
                    .OrderByDescending(x => x.UpdatedAt)
                    .ToList();

            var user = await GetUser();
            var result = new List<ProductGetDto>();
            foreach (var product in list)
            {
                var productDto = _mapper.Map<ProductGetDto>(product);

                // Check if the user has liked the product or not
                productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.User.UserId == user?.UserId);

                result.Add(productDto);
            }

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
                productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.User.UserId == user?.UserId);

                result.Add(productDto);
            }

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products/Review/2
        [HttpGet("GetReview/{id}")]
        public async Task<IActionResult> GetReview(Guid id)
        {
            var product = await _product.FindById(id);
            if (product == null || product.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var revews = await _productReview.FindByProductId(product.ProductId);
            revews = revews.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();

            var result = _mapper.Map<List<ProductReviewGetDto>>(revews);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products/GetById/5
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _product.FindById(id);

            if (product == null || product.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var result = _mapper.Map<ProductGetDto>(product);
            var user = await GetUser();
            result.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.User.UserId == user?.UserId);

            return Ok(new ApiResponse { Success = true, Data = result });
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
            result.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.User.UserId == user?.UserId);

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
