using System.ComponentModel.DataAnnotations;
using Utility;

namespace BE_TKDecor.Core.Dtos.ProductReviewInteraction
{
    public class ProductReviewInteractionDto
    {
        public Guid ProductReviewId { get; set; }

        [RegularExpression($"^({SD.InteractionLike}|{SD.InteractionDislike}|{SD.InteractionNormal})$")]
        public string Interaction { get; set; } = null!;
    }
}
