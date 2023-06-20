using AutoMapper;
using BE_TKDecor.Core.Dtos.Favorite;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductFavoriteRepository _productFavorite;

        public FavoritesController(IMapper mapper,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IProductFavoriteRepository productFavorite)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _productFavorite = productFavorite;
        }

        [HttpGet("GetFavoriteOfUser")]
        public async Task<IActionResult> GetFavoriteOfUser()
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var list = await _productFavorite.GetFavoriteOfUser(user.UserId);

            var result = _mapper.Map<List<FavoriteGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        [HttpPost("SetFavorite")]
        public async Task<IActionResult> SetFavorite(FavoriteSetDto favoriteDto)
        {
            var product = await _productRepository.FindById(favoriteDto.ProductId);
            if (product == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            // Get current information whether the user likes this product
            // if not then add
            // yes then delete
            var productFavoriteDb = await _productFavorite
                .FindProductFavorite(user.UserId, product.ProductId);
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
                    await _productFavorite.Delete(productFavoriteDb);
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
                    return await _userRepository.FindById(int.Parse(userId));
            }
            return null;
        }
    }
}
