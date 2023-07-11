using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ProductReviewInteraction
{
    public class ProductReviewInteractionDto
    {
        public Guid ProductReviewId { get; set; }

        [RegularExpression($"^(Like|DisLike|Normal)$")]
        public string Interaction { get; set; } = null!;
    }
}
