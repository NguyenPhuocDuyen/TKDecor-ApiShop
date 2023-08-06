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
        private ApiResponse _response;

        public ProductReviewInteractionService(TkdecorContext context)
        {
            _context = context;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> Interaction(Guid userId, ProductReviewInteractionDto interactionDto)
        {
            var productReview = await _context.ProductReviews.FindAsync(interactionDto.ProductReviewId);
            if (productReview == null || productReview.IsDelete)
            {
                _response.Message = ErrorContent.ProductReviewNotFound;
                return _response;
            }

            var interactionReview = await _context.ProductReviewInteractions.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.ProductReviewId == productReview.ProductReviewId);

            bool isAdd = false;

            if (interactionReview == null)
            {
                isAdd = true;
                interactionReview = new ProductReviewInteraction();
            }

            if (isAdd)
            {
                interactionReview.UserId = userId;
                interactionReview.ProductReviewId = productReview.ProductReviewId;
            }
            interactionReview.UpdatedAt = DateTime.Now;
            interactionReview.Interaction = interactionDto.Interaction;

            try
            {
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
            catch
            {
                _response.Message = ErrorContent.Data;
            }
            return _response;
        }
    }
}
