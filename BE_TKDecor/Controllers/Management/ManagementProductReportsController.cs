using AutoMapper;
using BE_TKDecor.Core.Dtos.ProductReport;
using BE_TKDecor.Core.Response;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.SD;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = $"{RoleContent.Admin},{RoleContent.Seller}")]
    public class ManagementProductReportsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductReportRepository _productReport;

        public ManagementProductReportsController(IMapper mapper,
            IProductReportRepository productReport
            )
        {
            _mapper = mapper;
            _productReport = productReport;
        }


        // GET: api/ManagementProductReports/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _productReport.GetAll();
            reports = reports.Where(x => !x.IsDelete)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();

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
            if (report == null || report.IsDelete)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReportNotFound });

            if (!Enum.TryParse<ReportStatus>(reportDto.ReportStatus, out ReportStatus status))
                return BadRequest(new ApiResponse { Message = ErrorContent.ReportStatusNotFound });

            report.ReportStatus = status;
            report.UpdatedAt = DateTime.Now;
            try
            {
                await _productReport.Update(report);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

    }
}
