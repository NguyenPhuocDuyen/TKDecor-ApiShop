using AutoMapper;
using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Customer)]
    public class FavoritesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _product;
        private readonly IUserRepository _user;
        private readonly IProductFavoriteRepository _productFavorite;

        public FavoritesController(IMapper mapper,
            IProductRepository product,
            IUserRepository user,
            IProductFavoriteRepository productFavorite)
        {
            _mapper = mapper;
            _product = product;
            _user = user;
            _productFavorite = productFavorite;
        }

        // GET: api/Favorites/GetFavoriteOfUser
        [HttpGet("GetFavoriteOfUser")]
        public async Task<IActionResult> GetFavoriteOfUser()
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var list = (await _productFavorite.FindByUserId(user.UserId))
                .Where(x => x.IsDelete == false)
                .OrderByDescending(x => x.UpdatedAt);

            var result = _mapper.Map<List<FavoriteGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Favorites/SetFavorite
        [HttpPost("SetFavorite")]
        public async Task<IActionResult> SetFavorite(FavoriteSetDto favoriteDto)
        {
            var product = await _product.FindById(favoriteDto.ProductId);
            if (product == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            // Get current information whether the user likes this product
            // if not then add
            // yes then delete
            var productFavoriteDb = await _productFavorite
                .FindByUserIdAndProductId(user.UserId, product.ProductId);
            try
            {
                if (productFavoriteDb == null)
                {
                    ProductFavorite newProductFavorite = new()
                    {
                        ProductId = product.ProductId,
                        UserId = user.UserId
                    };
                    await _productFavorite.Add(newProductFavorite);
                }
                else
                {
                    productFavoriteDb.IsDelete = !productFavoriteDb.IsDelete;
                    await _productFavorite.Update(productFavoriteDb);
                }
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
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
