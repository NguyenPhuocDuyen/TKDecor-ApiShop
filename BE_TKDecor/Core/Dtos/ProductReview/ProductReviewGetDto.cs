using BusinessObject;
using Utility.SD;

namespace BE_TKDecor.Core.Dtos.ProductReview
{
    public class ProductReviewGetDto : BaseEntity
    {
        public Guid ProductReviewId { get; set; }

        public string UserAvatarUrl { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public int Rate { get; set; }

        public string Description { get; set; } = null!;

        public int TotalLike { get; set; }

        public int TotalDisLike { get; set; }

        public string InteractionOfUser { get; set; } = Interaction.Normal.ToString();
    }
}
