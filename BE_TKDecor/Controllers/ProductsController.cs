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
using System.Net.WebSockets;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
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
        [AllowAnonymous]
        public async Task<IActionResult> FeaturedProducts()
        {
            var products = await _productRepository.GetAll();
            var sort = products
                .OrderByDescending(x => x.OrderDetails.Sum(x => x.Quantity))
                .Take(9)
                .ToList();

            var result = _mapper.Map<List<ProductGetDto>>(sort);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _productRepository.GetAll();
            list = list.Where(x => x.IsDelete is not true).ToList();
            var result = _mapper.Map<List<ProductGetDto>>(list);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products/5
        [HttpGet("GetById/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.FindById(id);

            if (product == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            if (product.IsDelete is true)
                return NotFound(new ApiResponse { Message = "Product has been removed!" });

            var result = _mapper.Map<ProductGetDto>(product);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products/5
        [HttpGet("GetBySlug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var product = await _productRepository.FindBySlug(slug);

            if (product == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var result = _mapper.Map<ProductGetDto>(product);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        //// PUT: api/Products/5
        //[HttpPut("UpdateProduct/{id}")]
        //public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto product)
        //{
        //    if (id != product.ProductId)
        //        return BadRequest(new ApiResponse { Message = ErrorContent.Error });

        //    var productDb = await _productRepository.FindById(id);
        //    if (productDb == null)
        //        return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

        //    var p = await _productRepository.FindByName(product.Name);
        //    if (p != null)
        //        return BadRequest(new ApiResponse { Message = "Đã tồn tại tên sản phẩm!" });

        //    var newSlug = Slug.GenerateSlug(product.Name);
        //    var proSlug = await _productRepository.FindBySlug(newSlug);
        //    if (proSlug != null && proSlug.ProductId != id)
        //        return BadRequest(new ApiResponse { Message = "Hãy đặt tên khác do trùng dữ liệu!" });

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
        //    catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        //}

        // POST: api/Products
        [HttpPost("Create")]
        public async Task<ActionResult<Product>> Create(ProductCreateDto productDto)
        {
            var p = await _productRepository.FindByName(productDto.Name);
            if (p != null)
                return BadRequest(new ApiResponse { Message = "Product name already exists!" });

            var newSlug = Slug.GenerateSlug(productDto.Name);
            var proSlug = await _productRepository.FindBySlug(newSlug);
            if (proSlug != null)
                return BadRequest(new ApiResponse { Message = "Please change the name due to duplicate data!" });

            Product newProduct = _mapper.Map<Product>(productDto);
            newProduct.Slug = newSlug;
            newProduct.ProductImages = new List<ProductImage>();

            //set image for product
            foreach (var urlImage in productDto.Images)
            {
                ProductImage productImage = new()
                {
                    Product = newProduct,
                    ImageUrl = urlImage,
                };
                newProduct.ProductImages.Add(productImage);
            }
            //Url3dModel = product.Url3dModel,
            try
            {
                await _productRepository.Add(newProduct);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/Products/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.FindById(id);
            if (product == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            product.IsDelete = true;
            product.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _productRepository.Update(product);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
