using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.Cart;
using Utility;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleCustomer)]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cart;
        private readonly IUserService _user;

        public CartsController(ICartService cart,
            IUserService user)
        {
            _cart = cart;
            _user = user;
        }

        // GET: api/Carts/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetCarts()
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _cart.GetCartsForUser(user.UserId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Carts/AddProductToCart
        [HttpPost("AddProductToCart")]
        public async Task<IActionResult> AddProductToCart(CartCreateDto cartDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _cart.AddProductToCart(user.UserId, cartDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Carts/UpdateQuantity/201
        [HttpPut("UpdateQuantity/{id}")]
        public async Task<IActionResult> UpdateQuantity(Guid id, CartUpdateDto cartDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _cart.UpdateQuantity(user.UserId, id, cartDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // DELETE api/Carts/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _cart.Delete(user.UserId, id);
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
