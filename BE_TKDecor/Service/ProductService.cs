using AutoMapper;
using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Core.Dtos.ProductReview;
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

        public ProductService(TkdecorContext context, IMapper mapper)
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

            try
            {
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

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
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
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> GetBySlug(Guid? userId, string slug)
        {
            var product = await GetProductBySlug(slug);

            if (product == null || product.Quantity == 0)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            try
            {
                var result = _mapper.Map<ProductGetDto>(product);
                result.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == userId);

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> FeaturedProducts(Guid? userId)
        {
            var products = await GetAllProducts();
            products = products.Where(x => x.Quantity > 0)
                    .OrderByDescending(x => x.OrderDetails.Sum(x => x.Quantity))
                    .Take(9)
                    .ToList();

            try
            {
                var result = new List<ProductGetDto>();
                foreach (var product in products)
                {
                    var productDto = _mapper.Map<ProductGetDto>(product);

                    // Check if the user has liked the product or not
                    productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == userId);

                    result.Add(productDto);
                }

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> GetAll()
        {
            var products = await GetAllProducts();
            products = products.Where(x => x.Quantity > 0).ToList();

            try
            {
                var result = _mapper.Map<List<ProductGetDto>>(products);

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> GetAll(Guid? userId, Guid? categoryId, string search, string sort, int pageIndex, int pageSize)
        {
            var list = await GetAllProducts();

            // filter categoryId
            if (categoryId != null)
            {
                list = list.Where(x => x.CategoryId == categoryId).ToList();
            }

            // filter search
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Name.ToLower().Trim().Contains(search.ToLower().Trim())
                || x.Description.ToLower().Trim().Contains(search.ToLower().Trim())
                || x.Category.Name.ToLower().Trim().Contains(search.ToLower().Trim())
                ).ToList();
            }

            try
            {
                // map dto
                var listProductGet = new List<ProductGetDto>();
                foreach (var product in list)
                {
                    var productDto = _mapper.Map<ProductGetDto>(product);

                    // Check if the user has liked the product or not
                    productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == userId);

                    listProductGet.Add(productDto);
                }

                // filter sort
                listProductGet = sort switch
                {
                    "price-high-to-low" => listProductGet.OrderByDescending(x => x.Price).ToList(),
                    "price-low-to-high" => listProductGet.OrderBy(x => x.Price).ToList(),
                    "average-rate" => listProductGet.OrderByDescending(x => x.AverageRate).ToList(),
                    _ => listProductGet.OrderByDescending(x => x.CreatedAt).ToList(),
                };

                PaginatedList<ProductGetDto> pagingProduct = PaginatedList<ProductGetDto>.CreateAsync(
                    listProductGet, pageIndex, pageSize);

                var result = new
                {
                    products = pagingProduct,
                    pagingProduct.PageIndex,
                    pagingProduct.TotalPages,
                    pagingProduct.TotalItem
                };

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> GetReview(Guid? userId, string slug, string sort, int pageIndex, int pageSize)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Slug == slug);
            if (product == null || product.IsDelete)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            var revews = await _context.ProductReviews.Include(x => x.User)
                .Include(x => x.ProductReviewInteractions)
                .Where(x => x.ProductId == product.ProductId && !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            try
            {
                var listReviewGetDto = new List<ProductReviewGetDto>();
                foreach (var review in revews)
                {
                    var reviewDto = _mapper.Map<ProductReviewGetDto>(review);

                    // Check if the user has liked the product or not
                    var interaction = review.ProductReviewInteractions.FirstOrDefault(pf => !pf.IsDelete && pf.UserId == userId);
                    if (interaction != null)
                    {
                        reviewDto.InteractionOfUser = interaction.Interaction.ToString();
                    }

                    listReviewGetDto.Add(reviewDto);
                }

                // filter sort
                listReviewGetDto = sort switch
                {
                    "rate-high-to-low" => listReviewGetDto.OrderByDescending(x => x.Rate).ToList(),
                    "rate-low-to-high" => listReviewGetDto.OrderBy(x => x.Rate).ToList(),
                    "rate-most-like" => listReviewGetDto.OrderByDescending(x => x.TotalLike).ToList(),
                    _ => listReviewGetDto.OrderByDescending(x => x.CreatedAt).ToList(),
                };

                PaginatedList<ProductReviewGetDto> pagingReviews = PaginatedList<ProductReviewGetDto>.CreateAsync(
                   listReviewGetDto, pageIndex, pageSize);

                var result = new
                {
                    reviews = pagingReviews,
                    pagingReviews.PageIndex,
                    pagingReviews.TotalPages,
                    pagingReviews.TotalItem
                };

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> RelatedProducts(Guid? userId, string slug)
        {
            var p = await GetProductBySlug(slug);

            if (p == null || p.Quantity == 0)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            var productList = await GetAllProducts();

            productList = productList.Where(x => x.ProductId != p.ProductId
                && x.CategoryId == p.CategoryId)
                .Take(5)
                .ToList();

            try
            {
            // map dto
            var listProductGet = new List<ProductGetDto>();
            foreach (var product in productList)
            {
                var productDto = _mapper.Map<ProductGetDto>(product);

                // Check if the user has liked the product or not
                productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId == userId);

                listProductGet.Add(productDto);
            }

            _response.Success = true;
            _response.Data = listProductGet;
            }
            catch { _response.Message = ErrorContent.Data; }
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
                            _context.ProductImages.Remove(imageOld);
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
                _context.Products.Update(productDb);
                await _context.SaveChangesAsync();

                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        private async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductReviews)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
        }

        private async Task<Product?> GetProductBySlug(string slug)
        {
            return await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductReviews)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .FirstOrDefaultAsync(x => x.Slug == slug);
        }
    }
}
