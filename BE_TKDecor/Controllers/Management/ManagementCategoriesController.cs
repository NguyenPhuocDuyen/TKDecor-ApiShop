using AutoMapper;
using BE_TKDecor.Core.Dtos.Category;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementCategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _category;

        public ManagementCategoriesController(IMapper mapper,
            ICategoryRepository category)
        {
            _mapper = mapper;
            _category = category;
        }

        // GET: api/ManagementCategories/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _category.GetAll();
            list = list.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = _mapper.Map<List<CategoryGetDto>>(list);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/ManagementCategories/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CategoryCreateDto categoryDto)
        {
            var categoryDb = await _category.FindByName(categoryDto.Name);
            bool isAdd = true;
            if (categoryDb == null)
            {
                categoryDb = _mapper.Map<Category>(categoryDto);
            }
            else
            {
                if (!categoryDb.IsDelete)
                    return BadRequest(new ApiResponse { Message = "Tên danh mục đã tồn tại!" });

                categoryDb.IsDelete = false;
                isAdd = false;

                categoryDb.Name = categoryDto.Name;
                categoryDb.Thumbnail = categoryDto.Thumbnail;
                categoryDb.UpdatedAt = DateTime.Now;
            }

            try
            {
                if (isAdd)
                {
                    await _category.Add(categoryDb);
                }
                else
                {
                    await _category.Update(categoryDb);
                }
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // GET: api/ManagementCategories/Update/1
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, CategoryUpdateDto categoryDto)
        {
            if (id != categoryDto.CategoryId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var categoryDb = await _category.FindById(categoryDto.CategoryId);
            if (categoryDb == null || categoryDb.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.CategoryNotFound });

            var categoryName = await _category.FindByName(categoryDto.Name);
            if (categoryName != null && categoryName.CategoryId != id)
                return BadRequest(new ApiResponse { Message = "Tên danh mục đã tồn tại!" });

            categoryDb.Name = categoryDto.Name;
            categoryDb.Thumbnail = categoryDto.Thumbnail;
            categoryDb.UpdatedAt = DateTime.Now;
            try
            {
                await _category.Update(categoryDb);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // GET: api/ManagementCategories/Delete/1
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var categoryDb = await _category.FindById(id);
            if (categoryDb == null || categoryDb.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.CategoryNotFound });

            categoryDb.Products = categoryDb.Products.Where(x => !x.IsDelete).ToList();

            if (categoryDb.Products.Count > 0)
                return BadRequest(new ApiResponse { Message = "Vẫn còn sản phẩm trong danh mục nên không thể xóa!" });

            categoryDb.IsDelete = true;
            categoryDb.UpdatedAt = DateTime.Now;
            try
            {
                await _category.Update(categoryDb);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
