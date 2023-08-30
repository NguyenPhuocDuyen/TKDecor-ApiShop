using BE_TKDecor.Core.Dtos.Product3DModel;
using BE_TKDecor.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ManagementProduct3DModelsController : ControllerBase
    {
        private readonly IProduct3DModelService _product3DModel;

        public ManagementProduct3DModelsController(IProduct3DModelService product3DModel)
        {
            _product3DModel = product3DModel;
        }

        // GET: api/ManagementProduct3DModels/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _product3DModel.GetAll();
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // GET: api/ManagementProduct3DModels/GetAllByProductId/1
        [HttpGet("GetAllByProductId/{id}")]
        public async Task<IActionResult> GetAllByProductId(Guid id)
        {
            var res = await _product3DModel.GetAllByProductId(id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/ManagementProduct3DModels/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Product3DModelCreateDto dto)
        {
            var res = await _product3DModel.Create(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/ManagementProduct3DModels/Delete/1
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _product3DModel.Delete(id);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
