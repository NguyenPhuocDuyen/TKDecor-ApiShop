using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleCustomer)]
    public class FavoritesController : Controller
    {
        private readonly IProductFavoriteService _productFavorite;
        private readonly IUserService _user;

        public FavoritesController(IProductFavoriteService productFavorite,
            IUserService user)
        {
            _productFavorite = productFavorite;
            _user = user;
        }

        // GET: api/Favorites/GetFavoriteOfUser
        [HttpGet("GetFavoriteOfUser")]
        public async Task<IActionResult> GetFavoriteOfUser(int pageIndex = 1, int pageSize = 20)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _productFavorite.GetFavoriteOfUser(user.UserId, pageIndex, pageSize);
            return Ok(res);
        }

        // GET: api/Favorites/SetFavorite
        [HttpPost("SetFavorite")]
        public async Task<IActionResult> SetFavorite(FavoriteSetDto favoriteDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _productFavorite.SetFavorite(user.UserId, favoriteDto);
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
