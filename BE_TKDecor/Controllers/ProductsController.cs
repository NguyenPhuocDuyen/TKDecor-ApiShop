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
        private readonly IProductImageRepository _productImageRepository;

        public ProductsController(IMapper mapper,
            IProductRepository productRepository,
            IProductImageRepository productImageRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
        }

        // GET: api/Products/GetAll
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _productRepository.GetAll();
            list = list.Where(x => x.IsDelete is not true)
                    .OrderByDescending(x => x.UpdatedAt)
                    .ToList();
            // paging skip 12*0 and take 12 after skip
            //list = list.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            var result = _mapper.Map<List<ProductGetDto>>(list);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Products/FeaturedProducts
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

        // GET: api/Products/GetById/5
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

        // GET: api/Products/GetBySlug/5
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

        // POST: api/Products/Create
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
            foreach (var urlImage in productDto.ProductImages)
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

        // PUT: api/Products/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto productDto)
        {
            if (id != productDto.ProductId)
                return BadRequest(new ApiResponse { Message = ErrorContent.Error });

            var productDb = await _productRepository.FindById(id);
            if (productDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var p = await _productRepository.FindByName(productDto.Name);
            if (p != null && p.ProductId != id)
                return BadRequest(new ApiResponse { Message = "Product name already exists!" });

            var newSlug = Slug.GenerateSlug(productDto.Name);
            var proSlug = await _productRepository.FindBySlug(newSlug);
            if (proSlug != null && proSlug.ProductId != id)
                return BadRequest(new ApiResponse { Message = "Please change the name due to duplicate data!" });

            productDb.CategoryId = productDto.CategoryId;
            productDb.Name = productDto.Name;
            productDb.Description = productDto.Description;
            productDb.Slug = newSlug;
            productDb.Quantity = productDto.Quantity;
            productDb.Price = productDto.Price;
            //productDb.Url3dModel = product.Url3dModel;
            productDb.UpdatedAt = DateTime.UtcNow;

            List<string> listImageUrlOld = productDb.ProductImages.Select(x => x.ImageUrl).ToList();
            try
            {

                // delete the old photo if it's not in the new photo list
                foreach (var imageUrlOld in listImageUrlOld)
                {
                    if (!productDto.ProductImages.Contains(imageUrlOld))
                    {
                        var imageOld = productDb.ProductImages.FirstOrDefault(x => x.ImageUrl == imageUrlOld);
                        if (imageOld != null)
                        {
                            productDb.ProductImages.Remove(imageOld);
                            await _productImageRepository.Delete(imageOld);
                        }
                    }
                }

                // add a new photo if it's not in the list of photos
                foreach (var imageUrlNew in productDto.ProductImages)
                {
                    if (!listImageUrlOld.Contains(imageUrlNew))
                    {
                        ProductImage imageNew = new()
                        {
                            ProductId = productDb.ProductId,
                            Product = productDb,
                            ImageUrl = imageUrlNew
                        };
                        productDb.ProductImages.Add(imageNew);
                    }
                }

                // Update information except photos
                await _productRepository.Update(productDb);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/Products/Delete/5
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
