using AutoMapper;
using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace BE_TKDecor.Service
{
    public class ProductService : IProductService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public ProductService(TkdecorContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> Create(ProductCreateDto dto)
        {
            if (dto.Product3DModelId != null)
            {
                var model = await _context.Product3Dmodels.FindAsync(dto.Product3DModelId);
                if (model == null || model.IsDelete)
                {
                    _response.Message = ErrorContent.Model3DNotFound;
                    return _response;
                }
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null || category.IsDelete)
            {
                _response.Message = ErrorContent.CategoryNotFound;
                return _response;
            }

            var newSlug = Slug.GenerateSlug(dto.Name);
            var productDb = await _context.Products.FirstOrDefaultAsync(x => x.Slug == newSlug);
            if (productDb != null)
                newSlug += new Random().Next(1000, 9999);

            var newProduct = new Product();
            newProduct = _mapper.Map<Product>(dto);
            newProduct.Slug = newSlug;
            newProduct.ProductImages = new List<ProductImage>();

            //set image for product
            foreach (var urlImage in dto.ProductImages)
            {
                ProductImage productImage = new()
                {
                    Product = newProduct,
                    ImageUrl = urlImage,
                };
                newProduct.ProductImages.Add(productImage);
            }

            try
            {
                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var product = await _context.Products
                    .Include(x => x.Carts)
                    .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null || product.IsDelete)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            product.IsDelete = true;
            product.UpdatedAt = DateTime.Now;
            foreach (var item in product.Carts)
            {
                item.IsDelete = true;
                item.UpdatedAt = DateTime.Now;
            }
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> GetAll()
        {
            var products = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductReviews)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            var result = _mapper.Map<List<ProductGetDto>>(products);

            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> Update(Guid id, ProductUpdateDto dto)
        {
            if (id != dto.ProductId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var productDb = await _context.Products
                    .Include(x => x.ProductImages)
                    .Include(x => x.Product3DModel)
                    .FirstOrDefaultAsync(x => x.ProductId == id);

            if (productDb == null || productDb.IsDelete)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            var newSlug = Slug.GenerateSlug(dto.Name);
            var proSlug = await _context.Products.FirstOrDefaultAsync(x => x.Slug == newSlug);
            if (proSlug != null)
                newSlug += new Random().Next(1000, 9999);

            if (!string.IsNullOrEmpty(dto.Product3DModelId.ToString()))
            {
                var model = await _context.Product3Dmodels.FindAsync((Guid)dto.Product3DModelId);
                if (model == null || model.IsDelete)
                {
                    _response.Message = ErrorContent.Model3DNotFound;
                    return _response;
                }

                productDb.Product3DModelId = model.Product3DModelId;
                productDb.Product3DModel = model;
            }
            else
            {
                productDb.Product3DModelId = null;
                productDb.Product3DModel = null;
            }

            productDb.CategoryId = dto.CategoryId;
            productDb.Name = dto.Name;
            productDb.Description = dto.Description;
            productDb.Slug = newSlug;
            productDb.Quantity = dto.Quantity;
            productDb.Price = dto.Price;
            productDb.UpdatedAt = DateTime.Now;

            List<string> listImageUrlOld = productDb.ProductImages.Select(x => x.ImageUrl).ToList();
            try
            {

                // delete the old photo if it's not in the new photo list
                foreach (var imageUrlOld in listImageUrlOld)
                {
                    if (!dto.ProductImages.Contains(imageUrlOld))
                    {
                        var imageOld = productDb.ProductImages.FirstOrDefault(x => x.ImageUrl == imageUrlOld);
                        if (imageOld != null)
                        {
                            productDb.ProductImages.Remove(imageOld);
                        }
                    }
                }

                // add a new photo if it's not in the list of photos
                foreach (var imageUrlNew in dto.ProductImages)
                {
                    if (!listImageUrlOld.Contains(imageUrlNew))
                    {
                        ProductImage imageNew = new()
                        {
                            ProductId = productDb.ProductId,
                            //Product = productDb,
                            ImageUrl = imageUrlNew
                        };
                        productDb.ProductImages.Add(imageNew);
                    }
                }

                // Update information except photos
                _context.Update(productDb);
                await _context.SaveChangesAsync();

                _response.Success = true;
            }
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }
    }
}
