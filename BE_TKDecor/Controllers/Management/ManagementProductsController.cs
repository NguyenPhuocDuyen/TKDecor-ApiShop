using BE_TKDecor.Core.Dtos.Product;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementProductsController : ControllerBase
    {
        private readonly IProductService _product;

        public ManagementProductsController(IProductService product)
        {
            _product = product;
        }

        // GET: api/ManagementProducts/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _product.GetAll();
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/ManagementProducts/GetBySlug
        [HttpGet("GetBySlug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var res = await _product.GetBySlug(null, slug);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/ManagementProducts/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var res = await _product.Create(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // PUT: api/ManagementProducts/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, ProductUpdateDto dto)
        {
            var res = await _product.Update(id, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // DELETE: api/ManagementProducts/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _product.Delete(id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
