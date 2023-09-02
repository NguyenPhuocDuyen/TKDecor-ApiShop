using BE_TKDecor.Core.Dtos.ProductReviewInteraction;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace BE_TKDecor.Service
{
    public class ProductReviewInteractionService : IProductReviewInteractionService
    {
        private readonly TkdecorContext _context;
        private readonly ApiResponse _response;

        public ProductReviewInteractionService(TkdecorContext context)
        {
            _context = context;
            _response = new ApiResponse();
        }

        // user make interaction
        public async Task<ApiResponse> Interaction(string? userId, ProductReviewInteractionDto dto)
        {
            if (userId is null)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            //var productReview = await _context.ProductReviews.FindAsync(dto.ProductReviewId);
            //if (productReview is null || productReview.IsDelete)
            //{
            //    _response.Message = ErrorContent.ProductReviewNotFound;
            //    return _response;
            //}

            var interactionReview = await _context.ProductReviewInteractions.FirstOrDefaultAsync(x =>
                x.UserId.ToString() == userId && x.ProductReviewId == dto.ProductReviewId);

            bool isAdd = false;
            if (interactionReview is null)
            {
                isAdd = true;
                interactionReview = new ProductReviewInteraction();
            }

            if (isAdd)
            {
                interactionReview.ProductReviewId = dto.ProductReviewId;
            }

            interactionReview.Interaction = dto.Interaction;
            interactionReview.UpdatedAt = DateTime.Now;
            try
            {
                interactionReview.UserId = Guid.Parse(userId);
                if (isAdd)
                {
                    _context.ProductReviewInteractions.Add(interactionReview);
                }
                else
                {
                    _context.ProductReviewInteractions.Update(interactionReview);
                }
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }
    }
}
