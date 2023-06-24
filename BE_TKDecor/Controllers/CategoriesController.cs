using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Response;
using AutoMapper;
using BE_TKDecor.Core.Dtos.Category;
using Microsoft.AspNetCore.Authorization;
using DataAccess.StatusContent;
using DataAccess.DAO;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IMapper mapper,
            ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        // GET: api/Categorys/GetAll
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _categoryRepository.GetAll();
            list = list.Where(x => x.IsDelete is not true)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();
            var result = _mapper.Map<List<CategoryGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/Categorys/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CategoryCreateDto categoryDto)
        {
            var categoryDb = await _categoryRepository.FindByName(categoryDto.Name);
            if (categoryDb != null)
                return BadRequest(new ApiResponse { Message = "Category name already exists!" });

            Category newCategory = _mapper.Map<Category>(categoryDto);
            try
            {
                await _categoryRepository.Add(newCategory);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // GET: api/Categorys/Update/1
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto categoryDto)
        {
            if (id != categoryDto.CategoryId)
                return NotFound(new ApiResponse { Message = ErrorContent.CategoryNotFound });

            var categoryDb = await _categoryRepository.FindById(categoryDto.CategoryId);
            if (categoryDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.CategoryNotFound });

            var categoryName = await _categoryRepository.FindByName(categoryDto.Name);
            if (categoryName != null && categoryName.CategoryId != id)
                return BadRequest(new ApiResponse { Message = "Category name already exists!" });

            categoryDb.Name = categoryDto.Name;
            categoryDb.ImageUrl = categoryDto.ImageUrl;
            categoryDb.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _categoryRepository.Update(categoryDb);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // GET: api/Categorys/Delete/1
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryDb = await _categoryRepository.FindById(id);
            if (categoryDb == null)
                return NotFound(new ApiResponse { Message = ErrorContent.CategoryNotFound });

            var checkProductHasInCategory = await _categoryRepository.CheckProductExistsByCateId(id);
            if (checkProductHasInCategory)
                return BadRequest(new ApiResponse { Message = "There are still products in the category that cannot be deleted" });

            categoryDb.IsDelete = true;
            categoryDb.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _categoryRepository.Update(categoryDb);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
