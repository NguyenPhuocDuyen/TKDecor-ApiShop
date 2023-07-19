using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.Product;
using AutoMapper;
using BE_TKDecor.Core.Dtos.ProductReview;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _product;
        private readonly IProductReviewRepository _productReview;

        public ProductsController(IMapper mapper,
            IProductRepository product,
            IProductReviewRepository productReview)
        {
            _mapper = mapper;
            _product = product;
            _productReview = productReview;
        }

        // GET: api/Products/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _product.GetAll();
            list = list.Where(x => !x.IsDelete && x.Quantity > 0)
                    .OrderByDescending(x => x.UpdatedAt)
                    .ToList();
            // paging skip 12*0 and take 12 after skip
            //list = list.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            var result = _mapper.Map<List<ProductGetDto>>(list);
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

            var result = _mapper.Map<List<ProductGetDto>>(products);
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
            return Ok(new ApiResponse { Success = true, Data = result });
        }
    }
}
