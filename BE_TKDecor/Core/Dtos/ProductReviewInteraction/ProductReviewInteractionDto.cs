using DataAccess.StatusContent;
using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ProductReviewInteraction
{
    public class ProductReviewInteractionDto
    {
        public long ProductReviewId { get; set; }

        [RegularExpression($"^({ProductInteractionStatusContent.Like}|{ProductInteractionStatusContent.DisLike}|{ProductInteractionStatusContent.Normal})$")]
        public string Interaction { get; set; } = null!;
    }
}
