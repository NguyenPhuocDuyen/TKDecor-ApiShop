using BusinessObject;
using Utility.SD;

namespace BE_TKDecor.Core.Dtos.ProductReview
{
    public class ProductReviewGetDto
    {
        public Guid ProductReviewId { get; set; }

        public string? UserAvatarUrl { get; set; }

        public string UserName { get; set; } = null!;

        public int Rate { get; set; }

        public string Description { get; set; } = null!;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDelete { get; set; } = false;

        public int Like { get; set; }

        public int DisLike { get; set; }

        //public virtual ICollection<ProductReviewInteractionGetDto> ProductReviewInteractions { get; set; } = new List<ProductReviewInteractionGetDto>();
    }

    //public class ProductReviewInteractionGetDto
    //{
    //    //public Guid ProductReviewInteractionId { get; set; }

    //    //public Guid ProductReviewId { get; set; }

    //    public string Interaction { get; set; } = null!;
    //}
}
