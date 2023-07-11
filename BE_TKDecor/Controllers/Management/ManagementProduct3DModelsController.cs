using AutoMapper;
using BE_TKDecor.Core.Dtos.Product3DModel;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class ManagementProduct3DModelsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProduct3DModelRepository _product3DModel;

        public ManagementProduct3DModelsController(IMapper mapper,
            IProduct3DModelRepository product3DModel)
        {
            _mapper = mapper;
            _product3DModel = product3DModel;
        }

        // GET: api/ManagementProduct3DModels/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var models = await _product3DModel.GetAll();
            models = models.OrderByDescending(x => x.UpdatedAt).ToList();
            var result = _mapper.Map<List<Product3DModelGetDto>>(models);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/ManagementProduct3DModels/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Product3DModelCreateDto modelDto)
        {
            var model = _mapper.Map<Product3DModel>(modelDto);
            try
            {
                await _product3DModel.Add(model);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/ManagementProduct3DModels/Delete
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _product3DModel.FindById(id);
            if (model == null)
                return NotFound(new ApiResponse { Message = ErrorContent.Model3DNotFound });

            if (model.Product != null)
                return BadRequest(new ApiResponse { Message = "Model3D being used by the product: " + model.Product.Name });

            model.IsDelete = true;
            model.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _product3DModel.Update(model);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
