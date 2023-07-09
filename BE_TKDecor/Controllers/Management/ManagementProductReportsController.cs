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
        private readonly IProductReportRepository _productReport;
        private readonly IReportStatusRepository _reportStatus;

        public ManagementProductReportsController(IMapper mapper,
            IProductReportRepository productReport,
            IReportStatusRepository reportStatus
            )
        {
            _mapper = mapper;
            _productReport = productReport;
            _reportStatus = reportStatus;
        }


        // GET: api/ManagementProductReports/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _productReport.GetAll();
            reports = reports.OrderByDescending(x => x.UpdatedAt).ToList();

            var result = _mapper.Map<List<ProductReportGetDto>>(reports);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/ManagementProductReports/UpdateStatusReport
        [HttpPut("UpdateStatusReport/{id}")]
        public async Task<IActionResult> UpdateStatusReport(Guid id, ProductReportUpdateDto reportDto)
        {
            if (id != reportDto.ProductReportId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var report = await _productReport.FindById(id);
            if (report == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReportNotFound });

            if (report.ReportStatus.Name != ReportStatusContent.Pending)
                return BadRequest(new ApiResponse { Message = "Product Report has been processed!" });

            var reportStatus = (await _reportStatus.GetAll())
                .FirstOrDefault(x => x.Name == reportDto.ReportStatus);
            if (reportStatus == null)
                return NotFound(new ApiResponse { Message = ErrorContent.Error });

            report.ReportStatusId = reportStatus.ReportStatusId;
            report.ReportStatus = reportStatus;
            report.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _productReport.Update(report);
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

    }
}
