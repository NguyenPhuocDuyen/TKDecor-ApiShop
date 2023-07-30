using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.Cart;
using Utility.SD;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Customer)]
    public class CartsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly ICartRepository _cart;
        private readonly IProductRepository _product;

        public CartsController(IMapper mapper,
            IUserRepository user,
            ICartRepository cart,
            IProductRepository product)
        {
            _mapper = mapper;
            _user = user;
            _cart = cart;
            _product = product;
        }

        // GET: api/Carts/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetCarts()
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var carts = (await _cart.FindCartsByUserId(user.UserId))
                    .Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();

            var result = _mapper.Map<List<CartGetDto>>(carts);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/Carts/AddProductToCart
        [HttpPost("AddProductToCart")]
        public async Task<IActionResult> AddProductToCart(CartCreateDto cartDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            // get current product info
            var product = await _product.FindById(cartDto.ProductId);
            if (product == null || product.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            // get product information in cart
            var cartDb = await _cart.FindByUserIdAndProductId(user.UserId, product.ProductId);

            // Variable check number of valid products
            bool quanlityIsValid = true;

            // Variable check add or update product
            bool isAdd = false;

            // If not, create a new one, if yes, add the quantity
            if (cartDb == null)
            {
                isAdd = true;
                cartDb = _mapper.Map<Cart>(cartDto);
                cartDb.IsDelete = false;
                cartDb.UserId = user.UserId;
            }
            else
            {
                cartDb.UpdatedAt = DateTime.Now;
                if (cartDb.IsDelete == true)
                {
                    cartDb.IsDelete = false;
                    cartDb.Quantity = cartDto.Quantity;
                }
                else
                {
                    // Existing old cart plus quantity
                    cartDb.Quantity += cartDto.Quantity;
                }
            }

            // Exceeded the number of existing products added
            if (cartDb.Quantity > product.Quantity)
            {
                quanlityIsValid = false;
                cartDb.Quantity = product.Quantity;
            }

            try
            {
                if (isAdd)
                {
                    await _cart.Add(cartDb);
                }
                else
                {
                    await _cart.Update(cartDb);
                }

                if (!quanlityIsValid)
                    return BadRequest(new ApiResponse { Message = "Vượt quá số lượng trong kho nhưng vẫn cộng tối đa!" });

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/Carts/UpdateQuantity/201
        [HttpPut("UpdateQuantity/{id}")]
        public async Task<IActionResult> UpdateQuantity(Guid id, CartUpdateDto cartDto)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var cartDb = await _cart.FindById(id);
            if (cartDb == null || cartDb.UserId != user.UserId || cartDb.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.CartNotFound });

            // Variable check number of valid products
            bool quanlityIsValid = true;

            cartDb.Quantity = cartDto.Quantity;
            cartDb.UpdatedAt = DateTime.Now;

            if (cartDb.Quantity > cartDb.Product.Quantity)
            {
                cartDb.Quantity = cartDb.Product.Quantity;
                quanlityIsValid = false;
            }

            try
            {
                await _cart.Update(cartDb);

                if (!quanlityIsValid)
                    return Ok(new ApiResponse { Success = true, Message = "Vượt quá số lượng trong kho nhưng vẫn cộng tối đa!" });

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data}); }
        }

        // DELETE api/Carts/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await GetUser();
            if (user == null || user.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            // Find and delete cart
            var cartDb = await _cart.FindById(id);
            if (cartDb == null || cartDb.UserId != user.UserId || cartDb.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.CartNotFound });

            cartDb.IsDelete = true;
            cartDb.UpdatedAt = DateTime.Now;
            try
            {
                await _cart.Update(cartDb);
                return Ok(new ApiResponse { Success = true });
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
