using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _category;

        public CategoriesController(ICategoryService category)
        {
            _category = category;
        }

        // GET: api/Categorys/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _category.GetAll();
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
