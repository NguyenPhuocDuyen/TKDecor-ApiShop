using AutoMapper;
using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class ManagementProductReportsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductReportRepository _productReportRepository;
        private readonly IReportStatusRepository _reportStatusRepository;

        public ManagementProductReportsController(IMapper mapper,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IProductReportRepository productReportRepository,
            IReportStatusRepository reportStatusRepository
            )
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _productReportRepository = productReportRepository;
            _reportStatusRepository = reportStatusRepository;
        }


        // GET: api/ManagementProductReports/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _productReportRepository.GetAll();
            reports = reports.OrderByDescending(x => x.UpdatedAt).ToList();

            var result = _mapper.Map<List<ProductReportGetDto>>(reports);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/ManagementProductReports/UpdateStatusReport
        [HttpPut("UpdateStatusReport/{id}")]
        public async Task<IActionResult> UpdateStatusReport(int id, ProductReportUpdateDto reportDto)
        {
            if (id != reportDto.ProductReportId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var report = await _productReportRepository.FindById(id);
            if (report == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReportNotFound });

            if (report.ReportStatus.Name != ReportStatusContent.Pending)
                return BadRequest(new ApiResponse { Message = "Product Report has been processed!" });

            var reportStatus = (await _reportStatusRepository.GetAll())
                .FirstOrDefault(x => x.Name == reportDto.ReportStatus);
            if (reportStatus == null)
                return NotFound(new ApiResponse { Message = ErrorContent.Error });

            report.ReportStatusId = reportStatus.ReportStatusId;
            report.ReportStatus = reportStatus;
            report.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _productReportRepository.Update(report);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

    }
}
