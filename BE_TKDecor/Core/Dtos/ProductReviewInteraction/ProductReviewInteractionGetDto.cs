namespace BE_TKDecor.Core.Dtos.ProductReviewInteraction
{
    public class ProductReviewInteractionGetDto
    {
        //public Guid ProductReviewInteractionId { get; set; }

        public Guid ProductReviewId { get; set; }

        public string Interaction { get; set; } = null!;
    }
}
