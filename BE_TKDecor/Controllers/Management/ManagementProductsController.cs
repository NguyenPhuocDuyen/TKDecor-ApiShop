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
    [Authorize(Roles = RoleContent.Admin)]
    public class ManagementProductsController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly IMapper _mapper;
        private readonly IProductRepository _product;
        private readonly IProductImageRepository _productImage;
        private readonly IProduct3DModelRepository _product3DModel;
        private readonly ICategoryRepository _category;
        private readonly ICartRepository _cart;

        public ManagementProductsController(IHubContext<NotificationHub> notificationHub,
            IMapper mapper,
            IProductRepository product,
            IProductImageRepository productImage,
            IProduct3DModelRepository product3DModel,
            ICategoryRepository category,
            ICartRepository cart)
        {
            _notificationHub = notificationHub;
            _mapper = mapper;
            _product = product;
            _productImage = productImage;
            _product3DModel = product3DModel;
            _category = category;
            _cart = cart;
        }

        // GET: api/Products/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _product.GetAll();
            products = products.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<ProductGetDto>>(products);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/Products/Create
        [HttpPost("Create")]
        public async Task<ActionResult<Product>> Create(ProductCreateDto productDto)
        {
            if (productDto.Product3DModelId != null)
            {
                var model = await _product3DModel.FindById((Guid)productDto.Product3DModelId);
                if (model == null || model.IsDelete)
                    return NotFound(new ApiResponse { Message = ErrorContent.Model3DNotFound });
            }

            var category = await _category.FindById(productDto.CategoryId);
            if (category == null)
                return NotFound(new ApiResponse { Message = ErrorContent.CategoryNotFound });

            //bool isAdd = true;

            var newSlug = Slug.GenerateSlug(productDto.Name);
            var productDb = await _product.FindBySlug(newSlug);
            if (productDb != null)
                newSlug += new Random().Next(1000, 9999);

            //{
            productDb = new Product();
            productDb = _mapper.Map<Product>(productDto);
            productDb.Slug = newSlug;
            productDb.ProductImages = new List<ProductImage>();
            //}
            //else
            //{
            //    if (!productDb.IsDelete)
            //        return BadRequest(new ApiResponse { Message = "Please change the name due to duplicate data!" });

            //    isAdd = false;
            //    productDb.IsDelete = false;
            //    productDb.CategoryId = category.CategoryId;
            //    productDb.Category = category;
            //    productDb.Product3DModelId = productDto.Product3DModelId;
            //    productDb.Name = productDto.Name;
            //    productDb.Description = productDto.Description;
            //    productDb.Quantity = productDto.Quantity;
            //    productDb.Price = productDto.Price;
            //}

            //List<string> listImageUrlOld = productDb.ProductImages.Select(x => x.ImageUrl).ToList();
            //try
            //{
            //    // delete the old photo if it's not in the new photo list
            //    foreach (var imageUrlOld in listImageUrlOld)
            //    {
            //        if (!productDto.ProductImages.Contains(imageUrlOld))
            //        {
            //            var imageOld = productDb.ProductImages.FirstOrDefault(x => x.ImageUrl == imageUrlOld);
            //            if (imageOld != null)
            //            {
            //                productDb.ProductImages.Remove(imageOld);
            //                await _productImage.Delete(imageOld);
            //            }
            //        }
            //    }

            //    // add a new photo if it's not in the list of photos
            //    foreach (var imageUrlNew in productDto.ProductImages)
            //    {
            //        if (!listImageUrlOld.Contains(imageUrlNew))
            //        {
            //            ProductImage imageNew = new()
            //            {
            //                ProductId = productDb.ProductId,
            //                Product = productDb,
            //                ImageUrl = imageUrlNew
            //            };
            //            productDb.ProductImages.Add(imageNew);
            //        }
            //    }
            //    if (isAdd)
            //    {
            //        await _product.Add(productDb);
            //    }
            //    else
            //    {
            //        await _product.Update(productDb);
            //    }
            //    return Ok(new ApiResponse { Success = true });
            //}
            //catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
            //catch (Exception ex) { }

            //set image for product
            foreach (var urlImage in productDto.ProductImages)
            {
                ProductImage productImage = new()
                {
                    Product = productDb,
                    ImageUrl = urlImage,
                };
                productDb.ProductImages.Add(productImage);
            }

            try
            {
                await _product.Add(productDb);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // PUT: api/Products/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, ProductUpdateDto productDto)
        {
            if (id != productDto.ProductId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var productDb = await _product.FindById(id);
            if (productDb == null || productDb.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            var newSlug = Slug.GenerateSlug(productDto.Name);
            var proSlug = await _product.FindBySlug(newSlug);
            if (proSlug != null)
                newSlug += new Random().Next(1000, 9999);
            //if (proSlug != null && proSlug.ProductId != id)
            //    return BadRequest(new ApiResponse { Message = "Please change the name due to duplicate data!" });

            if (!string.IsNullOrEmpty(productDto.Product3DModelId.ToString()))
            {
                var model = await _product3DModel.FindById((Guid)productDto.Product3DModelId);
                if (model == null || model.IsDelete)
                    return NotFound(new ApiResponse { Message = ErrorContent.Model3DNotFound });

                productDb.Product3DModelId = model.Product3DModelId;
                productDb.Product3DModel = model;
            }
            else
            {
                productDb.Product3DModelId = null;
                productDb.Product3DModel = null;
            }

            //if (model != null)
            //{
            //    productDb.Product3DModelId = model.Product3DModelId;
            //    productDb.Product3DModel = model;
            //}
            //else
            //{
            //    productDb.Product3DModel = null;
            //    productDb.Product3DModelId = null;
            //}
            productDb.CategoryId = productDto.CategoryId;
            productDb.Name = productDto.Name;
            productDb.Description = productDto.Description;
            productDb.Slug = newSlug;
            productDb.Quantity = productDto.Quantity;
            productDb.Price = productDto.Price;
            productDb.UpdatedAt = DateTime.Now;

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
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // DELETE: api/Products/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _product.FindById(id);
            if (product == null || product.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductNotFound });

            product.IsDelete = true;
            foreach (var item in product.Carts)
            {
                item.IsDelete = true;
                item.UpdatedAt = DateTime.Now;
                await _cart.Update(item);
            }
            product.UpdatedAt = DateTime.Now;
            try
            {
                await _product.Update(product);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
