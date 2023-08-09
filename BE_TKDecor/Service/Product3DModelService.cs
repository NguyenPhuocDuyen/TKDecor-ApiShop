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

        public async Task<ApiResponse> Create(Product3DModelCreateDto dto)
        {
            var model = _mapper.Map<Product3DModel>(dto);
            try
            {
                await _context.Product3Dmodels.AddAsync(model);
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
            var model = await _context.Product3Dmodels.Include(x => x.Product)
                        .FirstOrDefaultAsync(x => x.Product3DModelId == id);

            if (model == null || model.IsDelete)
            {
                _response.Message = ErrorContent.Model3DNotFound;
                return _response;
            }

            if (model.Product != null)
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
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }

        public async Task<ApiResponse> GetAll()
        {
            var models = await _context.Product3Dmodels.Include(x => x.Product).ToListAsync();
            models = models.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<Product3DModelGetDto>>(models);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }

        public async Task<ApiResponse> GetAllByProductId(Guid id)
        {
            var models = await _context.Product3Dmodels.Include(x => x.Product).ToListAsync();
            models = models.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();

            var product = await _context.Products.FindAsync(id);
            if (product != null && product.Product3DModelId != null)
            {
                models = models.Where(x => x.Product == null
                            || x.Product3DModelId == product.Product3DModelId)
                            .ToList();
            }
            else
            {
                models = models.Where(x => x.Product == null).ToList();
            }

            var result = _mapper.Map<List<Product3DModelGetDto>>(models);
            _response.Success = true;
            _response.Data = result;
            return _response;
        }
    }
}
