using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using Utility;
using Microsoft.AspNetCore.Authorization;
using DataAccess.StatusContent;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Core.Dtos.Product;
using AutoMapper;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductsController(IMapper mapper,
            IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        [HttpGet("FeaturedProducts")]
        public async Task<IActionResult> FeaturedProducts()
        {
            var products = await _productRepository.GetAll();
            var sort = products
                .OrderByDescending(x => x.OrderDetails.Sum(x => x.Quantity))
                .Take(9)
                .ToList();

            var result = _mapper.Map<List<ProductGetDto>>(sort);

            return Ok(new ApiResponse 
            { Success = true , Data = result });
        }

        //// GET: api/Products
        //[HttpGet("GetAll")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var list = await _productRepository.GetAll();
        //    list = list.Where(x => x.IsDelete is not true).ToList();
        //    return Ok(new ApiResponse<List<Product>> { Success = true, Data = list });
        //}

        //// GET: api/Products/5
        //[HttpGet("GetProductById/{id}")]
        //public async Task<IActionResult> GetProductById(int id)
        //{
        //    var product = await _productRepository.FindById(id);

        //    if (product == null)
        //        return NotFound(new ApiResponse<object> { Message = ErrorContent.ProductNotFound });

        //    if (product.IsDelete is true)
        //        return NotFound(new ApiResponse<object> { Message = "Sản phẩm đã bị xoá!" });

        //    return Ok(new ApiResponse<Product> { Success = true, Data = product });
        //}

        //// GET: api/Products/5
        //[HttpGet("GetProductBySlug/{slug}")]
        //public async Task<IActionResult> GetProductBySlug(string slug)
        //{
        //    var product = await _productRepository.FindBySlug(slug);

        //    if (product == null)
        //        return NotFound(new ApiResponse<object> { Message = ErrorContent.ProductNotFound });

        //    return Ok(new ApiResponse<Product> { Success = true, Data = product });
        //}

        //// PUT: api/Products/5
        //[HttpPut("UpdateProduct/{id}")]
        //[Authorize(Roles = RoleContent.Admin + "," + RoleContent.Seller)]
        //public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto product)
        //{
        //    if (id != product.ProductId)
        //        return BadRequest(new ApiResponse<object> { Message = ErrorContent.Error });

        //    var productDb = await _productRepository.FindById(id);
        //    if (productDb == null)
        //        return NotFound(new ApiResponse<object> { Message = ErrorContent.ProductNotFound });

        //    var p = await _productRepository.FindByName(product.Name);
        //    if (p != null)
        //        return BadRequest(new ApiResponse<object> { Message = "Đã tồn tại tên sản phẩm!" });

        //    var newSlug = Slug.GenerateSlug(product.Name);
        //    var proSlug = await _productRepository.FindBySlug(newSlug);
        //    if (proSlug != null && proSlug.ProductId != id)
        //        return BadRequest(new ApiResponse<object> { Message = "Hãy đặt tên khác do trùng dữ liệu!" });

        //    productDb.CategoryId = product.CategoryId;
        //    productDb.Name = product.Name;
        //    productDb.Description = product.Description;
        //    productDb.Slug = newSlug;
        //    productDb.Quantity = product.Quantity;
        //    productDb.Price = product.Price;
        //    //productDb.Url3dModel = product.Url3dModel;
        //    //productDb.ProductImages = product.ProductImages;
        //    productDb.UpdatedAt = DateTime.UtcNow;

        //    try
        //    {
        //        await _productRepository.Update(productDb);
        //        return NoContent();
        //    }
        //    catch
        //    {
        //        return BadRequest(new ApiResponse<object> { Message = ErrorContent.Data });
        //    }
        //}

        //// POST: api/Products
        //[HttpPost("AddProduct")]
        //[Authorize(Roles = RoleContent.Admin + "," + RoleContent.Seller)]
        //public async Task<ActionResult<Product>> AddProduct(ProductDto product)
        //{
        //    var p = await _productRepository.FindByName(product.Name);
        //    if (p != null)
        //        return BadRequest(new ApiResponse<object> { Message = "Đã tồn tại tên sản phẩm!" });

        //    var newSlug = Slug.GenerateSlug(product.Name);
        //    var proSlug = await _productRepository.FindBySlug(newSlug);
        //    if (proSlug != null)
        //        return BadRequest(new ApiResponse<object> { Message = "Hãy đặt tên khác do trùng dữ liệu!" });

        //    Product newProduct = new()
        //    {
        //        CategoryId = product.CategoryId,
        //        Name = product.Name,
        //        Description = product.Description,
        //        Slug = newSlug,
        //        Quantity = product.Quantity,
        //        Price = product.Price,
        //        //Url3dModel = product.Url3dModel,
        //        //ProductImages = product.ProductImages,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow,
        //        IsDelete = false
        //    };

        //    try
        //    {
        //        await _productRepository.Add(newProduct);
        //        return NoContent();
        //    }
        //    catch
        //    {
        //        return BadRequest(new ApiResponse<object> { Message = ErrorContent.Data });
        //    }

        //}

        //// DELETE: api/Products/5
        //[HttpDelete("DeleteProduct/{id}")]
        //[Authorize(Roles = RoleContent.Admin + "," + RoleContent.Seller)]
        //public async Task<IActionResult> DeleteProduct(int id)
        //{
        //    var product = await _productRepository.FindById(id);
        //    if (product == null)
        //        return NotFound(new ApiResponse<object> { Message = ErrorContent.ProductNotFound });

        //    product.IsDelete = true;
        //    product.UpdatedAt = DateTime.UtcNow;

        //    try
        //    {
        //        await _productRepository.Update(product);
        //        return NoContent();
        //    }
        //    catch
        //    {
        //        return BadRequest(new ApiResponse<object> { Message = ErrorContent.Data });
        //    }
        //}
    }
}
