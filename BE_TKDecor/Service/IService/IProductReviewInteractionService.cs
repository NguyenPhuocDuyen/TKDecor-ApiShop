using BE_TKDecor.Core.Dtos.ProductReviewInteraction;
using BE_TKDecor.Core.Response;

namespace BE_TKDecor.Service.IService
{
    public interface IProductReviewInteractionService
    {
        Task<ApiResponse> Interaction(string? userId, ProductReviewInteractionDto interactionDto);
    }
}
