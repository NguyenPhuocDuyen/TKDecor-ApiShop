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
        private ICategoryRepository repository = new CategoryRepository();
        // GET: api/<CategorysController>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await repository.GetAll();
            list = list.Where(x => x.IsDelete is not true).ToList();
            return Ok(new ApiResponse<List<Category>> { Success = true, Data = list });
        }
    }
}
