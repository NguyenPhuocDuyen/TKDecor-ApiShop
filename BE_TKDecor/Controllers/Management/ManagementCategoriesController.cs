using BE_TKDecor.Core.Dtos.Category;
using BE_TKDecor.Service.IService;
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
        private readonly ICategoryService _category;

        public ManagementCategoriesController(ICategoryService category)
        {
            _category = category;
        }

        // GET: api/ManagementCategories/GetAll
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

        // GET: api/ManagementCategories/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            var res = await _category.Create(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/ManagementCategories/Update/1
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, CategoryUpdateDto dto)
        {
            var res = await _category.Update(id, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/ManagementCategories/Delete/1
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _category.Delete(id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
