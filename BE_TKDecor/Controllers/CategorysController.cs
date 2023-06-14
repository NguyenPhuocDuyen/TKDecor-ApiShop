using BusinessObject;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.Other;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorysController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategorysController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: api/<CategorysController>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _categoryRepository.GetAll();
            list = list.Where(x => x.IsDelete is not true).ToList();
            return Ok(new ApiResponse<List<Category>> { Success = true, Data = list });
        }
    }
}
