using BE_TKDecor.Core.Dtos.ProductReviewInteraction;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.RoleCustomer)]
    public class ProductReviewInteractionsController : ControllerBase
    {
        private readonly IProductReviewInteractionService _interaction;
        private readonly IUserService _user;

        public ProductReviewInteractionsController(IProductReviewInteractionService interaction,
            IUserService user)
        {
            _interaction = interaction;
            _user = user;
        }

        // POST: api/ProductReviews/Interaction
        [HttpPost("Interaction")]
        public async Task<IActionResult> Interaction(ProductReviewInteractionDto interactionDto)
        {
            var user = await GetUser();
            if (user is null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            var res = await _interaction.Interaction(user.UserId, interactionDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.GetById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
