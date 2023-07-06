using BusinessObject;

namespace BE_TKDecor.Core.Dtos.ProductReview
{
    public class ProductReviewGetDto
    {
        public long ProductReviewId { get; set; }

        public string? UserAvatarUrl { get; set; }

        public string UserName { get; set; } = null!;

        public int Rate { get; set; }

        public string Description { get; set; } = null!;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDelete { get; set; } = false;

        public virtual ICollection<ProductReviewInteractionGetDto> ProductReviewInteractions { get; set; } = new List<ProductReviewInteractionGetDto>();
    }

    public class ProductReviewInteractionGetDto
    {
        public long ProductReviewInteractionId { get; set; }

        public long ProductReviewId { get; set; }

        //public long ProductInteractionStatusId { get; set; }

        public string Interaction { get; set; } = null!;
    }
}
