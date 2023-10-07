using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Service.IService;
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

        public FavoritesController(IProductFavoriteService productFavorite)
        {
            _productFavorite = productFavorite;
        }

        // GET: api/Favorites/GetFavoriteOfUser
        [HttpGet("GetFavoriteOfUser")]
        public async Task<IActionResult> GetFavoriteOfUser(int pageIndex = 1, int pageSize = 20)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _productFavorite.GetFavoriteOfUser(userId, pageIndex, pageSize);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/Favorites/SetFavorite
        [HttpPost("SetFavorite")]
        public async Task<IActionResult> SetFavorite(FavoriteSetDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _productFavorite.SetFavorite(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
