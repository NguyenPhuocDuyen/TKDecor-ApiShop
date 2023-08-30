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
        private readonly ApiResponse _response;

        public CategoryService(TkdecorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        // create category
        public async Task<ApiResponse> Create(CategoryCreateDto dto)
        {
            dto.Name = dto.Name.Trim();
            var categoryDb = await _context.Categories
                .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Name.ToLower());

            bool isAdd = true;
            if (categoryDb is null)
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

        // delete category
        public async Task<ApiResponse> Delete(Guid id)
        {
            var categoryDb = await _context.Categories
                .Include(x => x.Products.Where(x => !x.IsDelete))
                .FirstOrDefaultAsync(x => x.CategoryId == id && !x.IsDelete);

            if (categoryDb is null)
            {
                _response.Message = ErrorContent.CategoryNotFound;
                return _response;
            }

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

        // get all category
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

        // update category
        public async Task<ApiResponse> Update(Guid id, CategoryUpdateDto dto)
        {
            if (id != dto.CategoryId)
            {
                _response.Message = ErrorContent.NotMatchId;
                return _response;
            }

            var categoryDb = await _context.Categories.FindAsync(dto.CategoryId);
            if (categoryDb is null || categoryDb.IsDelete)
            {
                _response.Message = ErrorContent.CategoryNotFound;
                return _response;
            }

            categoryDb.Name = dto.Name.Trim();
            // check name already exists
            var categoryName = await _context.Categories
                .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Name.ToLower());

            if (categoryName is not null && categoryName.CategoryId != id)
            {
                _response.Message = "Tên danh mục đã tồn tại!";
                return _response;
            }

            //categoryDb.Name = dto.Name;
            categoryDb.Thumbnail = dto.Thumbnail;
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
