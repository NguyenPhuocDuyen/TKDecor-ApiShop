using BE_TKDecor.Core.Dtos.ProductReviewInteraction;
using BE_TKDecor.Service.IService;
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

        public ProductReviewInteractionsController(IProductReviewInteractionService interaction)
        {
            _interaction = interaction;
        }

        // POST: api/ProductReviews/Interaction
        [HttpPost("Interaction")]
        public async Task<IActionResult> Interaction(ProductReviewInteractionDto dto)
        {
            var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var res = await _interaction.Interaction(userId, dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
