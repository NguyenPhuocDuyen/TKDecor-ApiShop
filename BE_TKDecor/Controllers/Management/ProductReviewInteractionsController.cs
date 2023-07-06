using BE_TKDecor.Core.Dtos.ProductReviewInteraction;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using DataAccess.StatusContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Customer)]
    public class ProductReviewInteractionsController : ControllerBase
    {
        private readonly IUserRepository _user;
        private readonly IProductReviewRepository _productReview;
        private readonly IProductReviewInteractionRepository _interaction;
        private readonly IProductReviewInteractionStatusRepository _interactionStatus;

        public ProductReviewInteractionsController(
            IUserRepository user,
            IProductReviewRepository productReview,
            IProductReviewInteractionRepository interaction,
            IProductReviewInteractionStatusRepository interactionStatus)
        {
            _user = user;
            _productReview = productReview;
            _interaction = interaction;
            _interactionStatus = interactionStatus;
        }

        // POST: api/ProductReviews/Interaction
        [HttpPost]
        public async Task<IActionResult> Interaction(ProductReviewInteractionDto interactionDto)
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var productReview = await _productReview.FindById(interactionDto.ProductReviewId);
            if (productReview == null)
                return NotFound(new ApiResponse { Message = ErrorContent.ProductReviewNotFound });

            var interactionReview = await _interaction
                .FindByUserIdAndProductReviewId(user.UserId, interactionDto.ProductReviewId);

            var statuses = await _interactionStatus.GetAll();
            var status = statuses.FirstOrDefault(x => x.Name == interactionDto.Interaction);

            bool isAdd = false;
            if (status == null)
                return NotFound(new ApiResponse { Message = ErrorContent.InteractionNotFound });

            if (interactionReview == null)
            {
                isAdd = true;
                interactionReview = new ProductReviewInteraction();
            }

            if (isAdd)
            {
                interactionReview.UserId = user.UserId;
                interactionReview.User = user;
                interactionReview.ProductReviewId = productReview.ProductReviewId;
                interactionReview.ProductReview = productReview;
            }
            else
            {
                interactionReview.UpdatedAt = DateTime.UtcNow;
            }
            interactionReview.ProductInteractionStatusId = status.ProductReviewInteractionStatusId;
            interactionReview.ProductInteractionStatus = status;

            try
            {
                if (isAdd)
                {
                    await _interaction.Add(interactionReview);
                }
                else
                {
                    await _interaction.Update(interactionReview);
                }
                return NoContent();
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.FindById(int.Parse(userId));
            }
            return null;
        }
    }
}
