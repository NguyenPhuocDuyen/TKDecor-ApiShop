using AutoMapper;
using BE_TKDecor.Core.Dtos.ProductReviewInteraction;
using BE_TKDecor.Core.Response;
using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.SD;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleContent.Customer)]
    public class ProductReviewInteractionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly IProductReviewRepository _productReview;
        private readonly IProductReviewInteractionRepository _interaction;

        public ProductReviewInteractionsController(IMapper mapper,
            IUserRepository user,
            IProductReviewRepository productReview,
            IProductReviewInteractionRepository interaction)
        {
            _mapper = mapper;
            _user = user;
            _productReview = productReview;
            _interaction = interaction;
        }

        // GET: api/ProductReviews/GetInteraction
        [HttpGet("GetInteractionOfUser")]
        public async Task<IActionResult> GetAll()
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var productReviewInteraction = await _interaction.FindByUserId(user.UserId);
            productReviewInteraction = productReviewInteraction.Where(x => x.IsDelete == false).ToList();

            var result = _mapper.Map<List<ProductReviewInteractionGetDto>>(productReviewInteraction);
            return Ok(new ApiResponse { Success = true, Data = result });
        }

        // POST: api/ProductReviews/Interaction
        [HttpPost("Interaction")]
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

            Interaction status;
            if (!Enum.TryParse<Interaction>(interactionDto.Interaction, out status))
                return BadRequest(new ApiResponse { Message = ErrorContent.OrderStatusNotFound });

            bool isAdd = false;

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
            interactionReview.Interaction = status;

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
                    return await _user.FindById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
