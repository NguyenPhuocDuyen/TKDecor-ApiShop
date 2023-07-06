using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.ProductReview
{
    public class ProductReviewCreateDto
    {
        public long ProductId { get; set; }

        [Range(1, 5)]
        public int Rate { get; set; }

        public string Description { get; set; } = null!;
    }
}
