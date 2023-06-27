using AutoMapper;
using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class ManagementProductsController : ControllerBase
    {
        private readonly IHubContext<SignalRServer> _signalRHub;
        private readonly IMapper _mapper;
        private readonly IProductRepository _product;
        private readonly IProductImageRepository _productImage;

        public ManagementProductsController(IHubContext<SignalRServer> signalRHub,
            IMapper mapper,
            IProductRepository product,
            IProductImageRepository productImage)
        {
            _signalRHub = signalRHub;
            _mapper = mapper;
            _product = product;
            _productImage = productImage;
        }

        // POST: api/Products/Create
        [HttpPost("Create")]
        public async Task<ActionResult<Product>> Create(ProductCreateDto productDto)
        {
            var p = await _product.FindByName(productDto.Name);
            if (p != null)
                return BadRequest(new ApiResponse { Message = "Product name already exists!" });

            var newSlug = Slug.GenerateSlug(productDto.Name);
            var proSlug = await _product.FindBySlug(newSlug);
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
                await _product.Add(newProduct);
                await _signalRHub.Clients.All.SendAsync("LoadNotification");
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

            var productDb = await _product.FindById(id);
            if (productDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var p = await _product.FindByName(productDto.Name);
            if (p != null && p.ProductId != id)
                return BadRequest(new ApiResponse { Message = "Product name already exists!" });

            var newSlug = Slug.GenerateSlug(productDto.Name);
            var proSlug = await _product.FindBySlug(newSlug);
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
                            await _productImage.Delete(imageOld);
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
                await _product.Update(productDb);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/Products/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _product.FindById(id);
            if (product == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            product.IsDelete = true;
            product.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _product.Update(product);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
