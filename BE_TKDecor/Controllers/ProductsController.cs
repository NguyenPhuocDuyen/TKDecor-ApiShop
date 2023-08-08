using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _product;
        private readonly IUserService _user;

        public ProductsController(IProductService product,
            IUserService user)
        {
            _product = product;
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
            var user = await GetUser();
            var res = await _product.GetAll(user?.UserId, categoryId, search, sort, pageIndex, pageSize);
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
            var user = await GetUser();
            var res = await _product.FeaturedProducts(user?.UserId);
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
            var user = await GetUser();
            var res = await _product.GetReview(user?.UserId, slug, sort, pageIndex, pageSize);
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
            var user = await GetUser();
            var res = await _product.RelatedProducts(user?.UserId, slug);
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
            var user = await GetUser();
            var res = await _product.GetBySlug(user?.UserId, slug);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.GetById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
