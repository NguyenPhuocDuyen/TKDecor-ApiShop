using AutoMapper;
using BE_TKDecor.Core.Dtos.Cart;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Service
{
    public class CartService : ICartService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public CartService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> AddProductToCart(Guid userId, CartCreateDto dto)
        {
            // get current product info
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product is null || product.IsDelete)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            if (product.Quantity == 0)
            {
                _response.Message = "Sản phẩm đã hết số lượng trong kho.";
                return _response;
            }

            // get product information in cart
            var cartDb = await _context.Carts
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == product.ProductId);

            // Variable check number of valid products
            bool quanlityIsValid = true;

            // Variable check add or update product
            bool isAdd = false;

            // If not, create a new one, if yes, add the quantity
            if (cartDb is null)
            {
                isAdd = true;
                cartDb = _mapper.Map<Cart>(dto);
                cartDb.IsDelete = false;
                cartDb.UserId = userId;
            }
            else
            {
                cartDb.UpdatedAt = DateTime.Now;
                if (cartDb.IsDelete == true)
                {
                    cartDb.IsDelete = false;
                    cartDb.Quantity = dto.Quantity;
                }
                else
                {
                    // Existing old cart plus quantity
                    cartDb.Quantity += dto.Quantity;
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
                    _context.Carts.Add(cartDb);
                }
                else
                {
                    _context.Carts.Update(cartDb);
                }
                await _context.SaveChangesAsync();

                if (!quanlityIsValid)
                {
                    _response.Message = "Vượt quá số lượng trong kho nhưng vẫn thêm tối đa!";
                    return _response;
                }
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // delete cart by id
        public async Task<ApiResponse> Delete(Guid userId, Guid id)
        {
            // Find and delete cart
            var cartDb = await _context.Carts.FindAsync(id);
            if (cartDb is null || cartDb.UserId != userId || cartDb.IsDelete)
            {
                _response.Message = ErrorContent.CartNotFound;
                return _response;
            }

            cartDb.IsDelete = true;
            cartDb.UpdatedAt = DateTime.Now;
            try
            {
                _context.Carts.Update(cartDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // GetCartsForUser
        public async Task<ApiResponse> GetCartsForUser(Guid userId)
        {
            var carts = await _context.Carts
                    .Include(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                    .Include(x => x.Product)
                        .ThenInclude(x => x.Category)
                    .Where(x => x.UserId == userId && !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            try
            {
                // update quantity or delete cart when quantity of product in store is 0
                foreach (var cartItem in carts)
                {
                    if (cartItem.Quantity > cartItem.Product.Quantity)
                    {
                        if (cartItem.Product.Quantity == 0)
                        {
                            cartItem.IsDelete = true;
                        }
                        else
                        {
                            cartItem.Quantity = cartItem.Product.Quantity;
                        }
                        cartItem.UpdatedAt = DateTime.Now;
                        _context.Carts.Update(cartItem);
                    }
                }
                await _context.SaveChangesAsync();

                var result = _mapper.Map<List<CartGetDto>>(carts);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // update quantity in cart
        public async Task<ApiResponse> UpdateQuantity(Guid userId, Guid id, CartUpdateDto cartDto)
        {
            if (id != cartDto.CartId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var cartDb = await _context.Carts.Include(x => x.Product)
                    .FirstOrDefaultAsync(x => x.CartId == id && !x.IsDelete && x.UserId == userId);

            if (cartDb is null)
            {
                _response.Message = ErrorContent.CartNotFound;
                return _response;
            }

            cartDb.Quantity = cartDto.Quantity;
            cartDb.UpdatedAt = DateTime.Now;

            // Variable check number of valid products
            bool quanlityIsValid = true;
            if (cartDb.Quantity > cartDb.Product.Quantity)
            {
                cartDb.Quantity = cartDb.Product.Quantity;
                quanlityIsValid = false;
            }

            try
            {
                _context.Carts.Update(cartDb);
                await _context.SaveChangesAsync();

                if (!quanlityIsValid)
                {
                    _response.Message = "Vượt quá số lượng trong kho nhưng vẫn cộng tối đa!";
                    return _response;
                }
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
