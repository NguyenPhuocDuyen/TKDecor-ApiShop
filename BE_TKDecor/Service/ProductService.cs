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
        private readonly ApiResponse _response;

        public ProductService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        // create product
        public async Task<ApiResponse> Create(ProductCreateDto dto)
        {
            // find model if it not null
            if (dto.Product3DModelId is not null)
            {
                var model = await _context.Product3Dmodels.FindAsync(dto.Product3DModelId);
                if (model is null || model.IsDelete)
                {
                    _response.Message = ErrorContent.Model3DNotFound;
                    return _response;
                }
            }

            // find category
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category is null || category.IsDelete)
            {
                _response.Message = ErrorContent.CategoryNotFound;
                return _response;
            }

            dto.Name = dto.Name.Trim();
            // check already exists slug
            var newSlug = Slug.GenerateSlug(dto.Name);
            var productDb = await _context.Products.FirstOrDefaultAsync(x => x.Slug == newSlug);
            if (productDb is not null)
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

        // delete product by id
        public async Task<ApiResponse> Delete(Guid productId)
        {
            // find product
            var product = await _context.Products
                    .Include(x => x.Carts.Where(x => !x.IsDelete))
                    .Include(x => x.ProductReports.Where(x => !x.IsDelete))
                    .FirstOrDefaultAsync(x => x.ProductId == productId && !x.IsDelete);

            if (product is null)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            product.Product3DModelId = null;
            product.IsDelete = true;
            product.UpdatedAt = DateTime.Now;

            // delete all cart items related to the product
            foreach (var item in product.Carts)
            {
                item.IsDelete = true;
                item.UpdatedAt = DateTime.Now;
            }

            // delete all report related to the product
            foreach (var report in product.ProductReports)
            {
                report.IsDelete = true;
                report.UpdatedAt = DateTime.Now;
            }

            // delete all review and report review related to the product
            var productReview = await _context.ProductReviews
                .Include(x => x.OrderDetail)
                .Include(x => x.ReportProductReviews.Where(x => !x.IsDelete))
                .Where(x => x.OrderDetail.ProductId == productId)
                .ToListAsync();

            foreach (var review in productReview)
            {
                review.IsDelete = true;
                review.UpdatedAt = DateTime.Now;
                foreach (var reportReview in review.ReportProductReviews)
                {
                    reportReview.IsDelete = true;
                    reportReview.UpdatedAt = DateTime.Now;
                }
            }

            try
            {
                _context.ProductReviews.UpdateRange(productReview);
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get product by slug
        public async Task<ApiResponse> GetBySlug(string? userId, string slug)
        {
            var product = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.ProductReview)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .FirstOrDefaultAsync(x => x.Slug == slug.Trim());

            if (product is null)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            try
            {
                var result = _mapper.Map<ProductGetDto>(product);
                result.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId.ToString() == userId);

                // set total review and total rating
                int totalReviews = 0;
                int totalRating = 0;

                foreach (var orderDetail in product.OrderDetails)
                {
                    if (orderDetail.ProductReview is not null  && !orderDetail.ProductReview.IsDelete)
                    {
                        totalReviews++;
                        totalRating += orderDetail.ProductReview.Rate;
                    }
                }

                if (totalReviews > 0)
                {
                    double averageRating = Math.Round((double)totalRating / totalReviews, 2);
                    result.AverageRate = averageRating;
                }
                result.CountRate = totalReviews;

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get featured product
        public async Task<ApiResponse> FeaturedProducts(string? userId)
        {
            var products = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.ProductReview)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

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
                    productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId.ToString() == userId);

                    // set total review and total rating
                    int totalReviews = 0;
                    int totalRating = 0;

                    foreach (var orderDetail in product.OrderDetails)
                    {
                        if (orderDetail.ProductReview is not null  && !orderDetail.ProductReview.IsDelete)
                        {
                            totalReviews++;
                            totalRating += orderDetail.ProductReview.Rate;
                        }
                    }

                    if (totalReviews > 0)
                    {
                        double averageRating = Math.Round((double)totalRating / totalReviews, 2);
                        productDto.AverageRate = averageRating;
                    }
                    productDto.CountRate = totalReviews;

                    result.Add(productDto);
                }

                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get all product
        public async Task<ApiResponse> GetAll()
        {
            var products = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.ProductReview)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();


            try
            {
                var result = _mapper.Map<List<ProductGetDto>>(products);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get all product have filter
        public async Task<ApiResponse> GetAll(string? userId, Guid? categoryId, string search, string sort, int pageIndex, int pageSize)
        {
            var list = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.ProductReview)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            list = list.Where(x => x.Quantity > 0).ToList();

            // filter categoryId
            if (categoryId is not null)
            {
                list = list.Where(x => x.CategoryId == categoryId).ToList();
            }

            // filter search
            if (!string.IsNullOrEmpty(search))
            {
                search = Slug.GenerateSlug(search);

                list = list.Where(x => Slug.GenerateSlug(x.Name).Contains(search)
                || Slug.GenerateSlug(x.Description).Contains(search)
                || Slug.GenerateSlug(x.Category.Name).Contains(search)
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
                    productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId.ToString() == userId);

                    // set total review and total rating
                    int totalReviews = 0;
                    int totalRating = 0;

                    foreach (var orderDetail in product.OrderDetails)
                    {
                        if (orderDetail.ProductReview is not null  && !orderDetail.ProductReview.IsDelete)
                        {
                            totalReviews++;
                            totalRating += orderDetail.ProductReview.Rate;
                        }
                    }

                    if (totalReviews > 0)
                    {
                        double averageRating = Math.Round((double)totalRating / totalReviews, 2);
                        productDto.AverageRate = averageRating;
                    }
                    productDto.CountRate = totalReviews;

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

        // get review of product by produc slug
        public async Task<ApiResponse> GetReview(string? userId, string slug, string sort, int pageIndex, int pageSize)
        {
            //var product = await _context.Products.FirstOrDefaultAsync(x => x.Slug == slug.Trim() && !x.IsDelete);
            //if (product is null)
            //{
            //    _response.Message = ErrorContent.ProductNotFound;
            //    return _response;
            //}

            var revews = await _context.ProductReviews
                    .Include(x => x.OrderDetail)
                        .ThenInclude(x => x.Order)
                            .ThenInclude(x => x.User)
                    .Include(x => x.OrderDetail)
                        .ThenInclude(x => x.Product)
                    .Include(x => x.ProductReviewInteractions)
                    .Where(x => x.OrderDetail.Product.Slug == slug.ToLower().Trim() && !x.IsDelete)
                    .ToListAsync();

            try
            {
                var listReviewGetDto = new List<ProductReviewGetDto>();
                foreach (var review in revews)
                {
                    var reviewDto = _mapper.Map<ProductReviewGetDto>(review);

                    // Check if the user has liked the product or not
                    var interaction = review.ProductReviewInteractions.FirstOrDefault(pf => !pf.IsDelete && pf.UserId.ToString() == userId);
                    if (interaction is not null)
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

        // get relate products by product slug
        public async Task<ApiResponse> RelatedProducts(string? userId, string slug)
        {
            var p = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.ProductReview)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .FirstOrDefaultAsync(x => x.Slug == slug.Trim());

            if (p is null)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            var productList = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.OrderDetails)
                    .Include(x => x.Product3DModel)
                    .Include(x => x.ProductImages)
                    .Include(x => x.OrderDetails)
                        .ThenInclude(x => x.ProductReview)
                    .Include(x => x.ProductFavorites)
                    .Where(x => !x.IsDelete)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            // take 5 related products, if not enough, get more products
            productList = productList.Where(x => x.ProductId != p.ProductId && x.Quantity > 0)
                .OrderByDescending(x => x.Category == p.Category)
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
                    productDto.IsFavorite = product.ProductFavorites.Any(pf => !pf.IsDelete && pf.UserId.ToString() == userId);

                    // set total review and total rating
                    int totalReviews = 0;
                    int totalRating = 0;

                    foreach (var orderDetail in product.OrderDetails)
                    {
                        if (orderDetail.ProductReview is not null && !orderDetail.ProductReview.IsDelete)
                        {
                            totalReviews++;
                            totalRating += orderDetail.ProductReview.Rate;
                        }
                    }

                    if (totalReviews > 0)
                    {
                        double averageRating = Math.Round((double)totalRating / totalReviews, 2);
                        productDto.AverageRate = averageRating;
                    }
                    productDto.CountRate = totalReviews;

                    listProductGet.Add(productDto);
                }

                _response.Success = true;
                _response.Data = listProductGet;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // update product
        public async Task<ApiResponse> Update(Guid productId, ProductUpdateDto dto)
        {
            if (productId != dto.ProductId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var productDb = await _context.Products
                    .Include(x => x.ProductImages)
                    .Include(x => x.Product3DModel)
                    .FirstOrDefaultAsync(x => x.ProductId == productId && !x.IsDelete);

            if (productDb is null)
            {
                _response.Message = ErrorContent.ProductNotFound;
                return _response;
            }

            dto.Name = dto.Name.Trim();
            // check slug already exists
            var newSlug = Slug.GenerateSlug(dto.Name);
            var proSlug = await _context.Products.FirstOrDefaultAsync(x => x.Slug == newSlug);
            if (proSlug is not null)
                newSlug += new Random().Next(1000, 9999);

            if (dto.Product3DModelId is not null)
            {
                var model = await _context.Product3Dmodels.FindAsync((Guid)dto.Product3DModelId);
                if (model is null || model.IsDelete)
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
                        if (imageOld is not null)
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
    }
}
