using AutoMapper;
using BE_TKDecor.Core.Dtos.Category;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly TkdecorContext _context;
        private readonly IMapper _mapper;
        private ApiResponse _response;

        public CategoryService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> Create(CategoryCreateDto dto)
        {
            var categoryDb = await _context.Categories.FirstOrDefaultAsync(x => x.Name == dto.Name);
            bool isAdd = true;
            if (categoryDb == null)
            {
                categoryDb = _mapper.Map<Category>(dto);
            }
            else
            {
                if (!categoryDb.IsDelete)
                {
                    _response.Message = "Tên danh mục đã tồn tại!";
                    return _response;
                }

                categoryDb.IsDelete = false;
                isAdd = false;

                categoryDb.Name = dto.Name;
                categoryDb.Thumbnail = dto.Thumbnail;
                categoryDb.UpdatedAt = DateTime.Now;
            }

            try
            {
                if (isAdd)
                {
                    _context.Categories.Add(categoryDb);
                }
                else
                {
                    _context.Categories.Update(categoryDb);
                }
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            var categoryDb = await _context.Categories
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.CategoryId == id);
            if (categoryDb == null || categoryDb.IsDelete)
            {
                _response.Message = ErrorContent.CategoryNotFound;
                return _response;
            }

            categoryDb.Products = categoryDb.Products.Where(x => !x.IsDelete).ToList();

            if (categoryDb.Products.Count > 0)
            {
                _response.Message = "Vẫn còn sản phẩm trong danh mục nên không thể xóa!";
                return _response;
            }

            categoryDb.IsDelete = true;
            categoryDb.UpdatedAt = DateTime.Now;

            try
            {
                _context.Categories.Update(categoryDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> GetAll()
        {
            var list = await _context.Categories.ToListAsync();
            list = list.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            try
            {
                var result = _mapper.Map<List<CategoryGetDto>>(list);
                _response.Success = true;
                _response.Data = result;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> Update(Guid id, CategoryUpdateDto categoryDto)
        {
            if (id != categoryDto.CategoryId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var categoryDb = await _context.Categories.FindAsync(categoryDto.CategoryId);
            if (categoryDb == null || categoryDb.IsDelete)
            {
                _response.Message = ErrorContent.CategoryNotFound;
                return _response;
            }

            var categoryName = await _context.Categories.FirstOrDefaultAsync(x => x.Name == categoryDto.Name);
            if (categoryName != null && categoryName.CategoryId != id)
            {
                _response.Message = "Tên danh mục đã tồn tại!";
                return _response;
            }

            categoryDb.Name = categoryDto.Name;
            categoryDb.Thumbnail = categoryDto.Thumbnail;
            categoryDb.UpdatedAt = DateTime.Now;
            try
            {
                _context.Categories.Update(categoryDb);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
