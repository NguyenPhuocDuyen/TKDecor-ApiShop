using AutoMapper;
using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Utility;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class ManagementProductsController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly IMapper _mapper;
        private readonly IProductRepository _product;
        private readonly IProductImageRepository _productImage;
        private readonly IProduct3DModelRepository _product3DModel;

        public ManagementProductsController(IHubContext<NotificationHub> notificationHub,
            IMapper mapper,
            IProductRepository product,
            IProductImageRepository productImage,
            IProduct3DModelRepository product3DModel)
        {
            _notificationHub = notificationHub;
            _mapper = mapper;
            _product = product;
            _productImage = productImage;
            _product3DModel = product3DModel;
        }

        // GET: api/Products/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _product.GetAll();
            products = products.OrderByDescending(x => x.UpdatedAt).ToList();
            var result = _mapper.Map<List<ProductGetDto>>(products);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/Products/Create
        [HttpPost("Create")]
        public async Task<ActionResult<Product>> Create(ProductCreateDto productDto)
        {
            var productDb = await _product.FindByName(productDto.Name);
            if (productDb != null)
                return BadRequest(new ApiResponse { Message = "Product name already exists!" });

            var newSlug = Slug.GenerateSlug(productDto.Name);
            var proSlug = await _product.FindBySlug(newSlug);
            if (proSlug != null)
                return BadRequest(new ApiResponse { Message = "Please change the name due to duplicate data!" });

            Product3DModel? model = new();
            if (productDto.Product3DModelId != null)
            {
                model = await _product3DModel.FindById((Guid)productDto.Product3DModelId);
                if (model != null)
                    return NotFound(new ApiResponse { Message = ErrorContent.Model3DNotFound });
            }

            Product newProduct = _mapper.Map<Product>(productDto);
            newProduct.Slug = newSlug;
            newProduct.ProductImages = new List<ProductImage>();
            if (model != null)
            {
                newProduct.Product3DModelId = model.Product3DModelId;
                newProduct.Product3DModel = model;
            }
            else
            {
                newProduct.Product3DModel = null;
                newProduct.Product3DModelId = null;
            }

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
                //await _notificationHub.Clients.All.SendAsync("LoadNotification");
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // PUT: api/Products/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, ProductUpdateDto productDto)
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

            Product3DModel? model = new();
            if (productDto.Product3DModelId != null)
            {
                model = await _product3DModel.FindById((Guid)productDto.Product3DModelId);
                if (model == null)
                    return NotFound(new ApiResponse { Message = ErrorContent.Model3DNotFound });
            }
            if (model != null)
            {
                productDb.Product3DModelId = model.Product3DModelId;
                productDb.Product3DModel = model;
            }
            else
            {
                productDb.Product3DModel = null;
                productDb.Product3DModelId = null;
            }

            productDb.CategoryId = productDto.CategoryId;
            productDb.Name = productDto.Name;
            productDb.Description = productDto.Description;
            productDb.Slug = newSlug;
            productDb.Quantity = productDto.Quantity;
            productDb.Price = productDto.Price;
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
        public async Task<IActionResult> Delete(Guid id)
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
