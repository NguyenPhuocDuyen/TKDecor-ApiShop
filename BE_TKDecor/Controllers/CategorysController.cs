using BusinessObject;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorysController : ControllerBase
    {
        private ICategoryRepository repository = new CategoryRepository();
        // GET: api/<CategorysController>
        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategorys() => repository.GetCategories();
    }
}
