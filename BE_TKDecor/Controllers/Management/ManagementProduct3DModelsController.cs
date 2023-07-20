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
    //[Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class ManagementProduct3DModelsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProduct3DModelRepository _product3DModel;
        private readonly IProductRepository _product;

        public ManagementProduct3DModelsController(IMapper mapper,
            IProduct3DModelRepository product3DModel,
            IProductRepository product)
        {
            _mapper = mapper;
            _product3DModel = product3DModel;
            _product = product;
        }

        // GET: api/ManagementProduct3DModels/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var models = await _product3DModel.GetAll();
            models = models.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();

            var result = _mapper.Map<List<Product3DModelGetDto>>(models);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // GET: api/ManagementProduct3DModels/GetAllByProductId
        [HttpGet("GetAllByProductId/{id}")]
        public async Task<IActionResult> GetAllByProductId(Guid id)
        {
            var models = await _product3DModel.GetAll();
            models = models.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();

            var product = await _product.FindById(id);
            if (product != null && product.Product3DModelId != null)
            {
                models = models.Where(x => x.Product == null 
                            || x.Product3DModelId == product.Product3DModelId)
                    .ToList();
            } else
            {
                models = models.Where(x => x.Product == null).ToList();
            }

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
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/ManagementProduct3DModels/Delete
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _product3DModel.FindById(id);
            if (model == null || model.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.Model3DNotFound });

            if (model.Product != null)
                return BadRequest(new ApiResponse { Message = "Model3D being used by the product: " + model.Product.Name });

            model.IsDelete = true;
            model.UpdatedAt = DateTime.Now;
            try
            {
                await _product3DModel.Update(model);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
