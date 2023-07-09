using AutoMapper;
using BE_TKDecor.Core.Dtos.ReportProductReview;
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
    public class ManagementReportProductReviewsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReportProductReviewRepository _reportProductReview;
        private readonly IReportStatusRepository _reportStatus;
        private readonly IProductReviewRepository _productReview;

        public ManagementReportProductReviewsController(IMapper mapper,
            IReportProductReviewRepository reportProductReview,
            IReportStatusRepository reportStatus,
            IProductReviewRepository productReview)
        {
            _mapper = mapper;
            _reportProductReview = reportProductReview;
            _reportStatus = reportStatus;
            _productReview = productReview;
        }

        // GET: api/ManagementReportProductReviews/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _reportProductReview.GetAll();
            reports = reports.OrderByDescending(x => x.UpdatedAt).ToList();

            var result = _mapper.Map<List<ReportProductReviewGetDto>>(reports);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // PUT: api/ManagementReportProductReviews/UpdateStatusReport
        [HttpPut("UpdateStatusReport/{id}")]
        public async Task<IActionResult> UpdateStatusReport(Guid id, ReportProductReviewUpdateDto reportDto)
        {
            if (id != reportDto.ReportProductReviewId)
                return BadRequest(new ApiResponse { Message = ErrorContent.NotMatchId });

            var report = await _reportProductReview.FindById(id);
            if (report == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ReportProductReviewNotFound });

            if (report.ReportStatus.Name != ReportStatusContent.Pending)
                return BadRequest(new ApiResponse { Message = "Report product review has been processed!" });

            var reportStatus = (await _reportStatus.GetAll())
                .FirstOrDefault(x => x.Name == reportDto.ReportStatus);
            if (reportStatus == null)
                return NotFound(new ApiResponse { Message = ErrorContent.Error });

            report.ReportStatusId = reportStatus.ReportStatusId;
            report.ReportStatus = reportStatus;
            report.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _reportProductReview.Update(report);

                // update product review to delete
                var productReview = await _productReview.FindById(report.ProductReviewReportedId);
                if (productReview != null)
                {
                    productReview.UpdatedAt = DateTime.UtcNow;
                    productReview.IsDelete = true;
                    await _productReview.Update(productReview);
                }

                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }
    }
}
