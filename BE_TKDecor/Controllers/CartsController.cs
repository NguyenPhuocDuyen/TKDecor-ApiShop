using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        public CartsController(ICartService cart)
        {
            _cart = cart;
        }

        // GET: api/Carts/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetCarts()
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _cart.GetCarts(userId);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Carts/AddProductToCart
        [HttpPost("AddProductToCart")]
        public async Task<IActionResult> AddProductToCart(CartCreateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _cart.AddProductToCart(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Carts/UpdateQuantity/201
        [HttpPut("UpdateQuantity/{id}")]
        public async Task<IActionResult> UpdateQuantity(Guid id, CartUpdateDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _cart.UpdateQuantity(userId, id, dto);
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
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _cart.Delete(userId, id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
