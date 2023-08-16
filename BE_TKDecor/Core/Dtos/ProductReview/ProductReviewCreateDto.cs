using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ProductReview
{
    public class ProductReviewCreateDto
    {
        public Guid ProductId { get; set; }

        [Range(1, 5)]
        public int Rate { get; set; }

        [MinLength(5)]
        [MaxLength(255)]
        public string Description { get; set; } = null!;
    }
}
