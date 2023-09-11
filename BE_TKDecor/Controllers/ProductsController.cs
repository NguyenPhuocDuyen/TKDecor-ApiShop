using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _product;

        public ProductsController(IProductService product)
        {
            _product = product;
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
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _product.GetAll(userId, categoryId, search, sort, pageIndex, pageSize);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Products/FeaturedProducts
        [HttpGet("FeaturedProducts")]
        public async Task<IActionResult> FeaturedProducts()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _product.FeaturedProducts(userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
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
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _product.GetReview(userId, slug, sort, pageIndex, pageSize);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Products/RelatedProducts/slug
        [HttpGet("RelatedProducts/{slug}")]
        public async Task<IActionResult> RelatedProducts(string slug)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _product.RelatedProducts(userId, slug);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Products/GetBySlug/5
        [HttpGet("GetBySlug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _product.GetBySlug(userId, slug);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
