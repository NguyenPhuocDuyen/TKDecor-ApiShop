using AutoMapper;
using BE_TKDecor.Core.Dtos.Product3DModel;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Service
{
    public class Product3DModelService : IProduct3DModelService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public Product3DModelService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        // create model 3d
        public async Task<ApiResponse> Create(Product3DModelCreateDto dto)
        {
            try
            {
                dto.ModelName = dto.ModelName.Trim();
                var model = _mapper.Map<Product3DModel>(dto);
                await _context.Product3Dmodels.AddAsync(model);
                await _context.SaveChangesAsync();

                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // delete model 3d
        public async Task<ApiResponse> Delete(Guid id)
        {
            var model = await _context.Product3Dmodels
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Product3DModelId == id && !x.IsDelete);

            if (model is null)
            {
                _response.Message = ErrorContent.Model3DNotFound;
                return _response;
            }

            // can't delete because it used by one product
            if (model.Product is not null)
            {
                _response.Message = "Model3D đang được sản phẩm sử dụng bởi " + model.Product.Name;
                return _response;
            }

            model.IsDelete = true;
            model.UpdatedAt = DateTime.Now;
            try
            {
                _context.Product3Dmodels.Update(model);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get all model 3d
        public async Task<ApiResponse> GetAll()
        {
            var models = await _context.Product3Dmodels.Include(x => x.Product)
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            try
            {
                var result = _mapper.Map<List<Product3DModelGetDto>>(models);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        // get all model 3d for update product
        public async Task<ApiResponse> GetAllByProductId(Guid id)
        {
            // get models that are not used or used by the product itself
            var models = await _context.Product3Dmodels
                .Include(x => x.Product)
                .Where(x => !x.IsDelete && (x.Product == null || x.Product.ProductId == id))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            //var product = await _context.Products.FindAsync(id);
            //if (product is not null && product.Product3DModelId is not null)
            //{
            //    models = models.Where(x => x.Product is null
            //                || x.Product3DModelId == product.Product3DModelId)
            //                .ToList();
            //}
            //else
            //{
            //    models = models.Where(x => x.Product is null).ToList();
            //}

            try
            {
                var result = _mapper.Map<List<Product3DModelGetDto>>(models);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
