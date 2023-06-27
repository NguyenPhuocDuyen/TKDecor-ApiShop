using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Response;
using AutoMapper;
using BE_TKDecor.Core.Dtos.Category;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetAll()
        {
            var list = await _categoryRepository.GetAll();
            list = list.Where(x => x.IsDelete is not true)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();
            var result = _mapper.Map<List<CategoryGetDto>>(list);

            return Ok(new ApiResponse { Success = true, Data = result });
        }
    }
}
