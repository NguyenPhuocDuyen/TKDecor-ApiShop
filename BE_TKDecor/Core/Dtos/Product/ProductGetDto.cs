﻿using BE_TKDecor.Core.Dtos.ProductReview;

namespace BE_TKDecor.Core.Dtos.Product
{
    public class ProductGetDto
    {
        public long ProductId { get; set; }

        //public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public long? Product3DModelId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Slug { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public float AverageRate { get; set; }

        public int CountRate { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDelete { get; set; } = false;

        public List<string> ProductImages { get; set; } = new List<string>();

        //public virtual ICollection<ProductReviewGetDto> ProductReviews { get; set; } = new List<ProductReviewGetDto>();
    }

    //public class ProductReviewGetDto
    //{
    //    public long ProductReviewId { get; set; }

    //    public string? UserAvatarUrl { get; set; }

    //    public string UserName { get; set; } = null!;

    //    public int Rate { get; set; }

    //    public string Description { get; set; } = null!;

    //    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    //    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    //    public bool? IsDelete { get; set; } = false;

    //    //public virtual ICollection<ProductReviewInteraction> ProductReviewInteractions { get; set; } = new List<ProductReviewInteraction>();
    //}
}
