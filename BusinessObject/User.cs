using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public bool? IsSubscriber { get; set; } = false;

    public bool? EmailConfirmed { get; set; } = false;

    public string? EmailConfirmationCode { get; set; }

    public DateTime? EmailConfirmationSentAt { get; set; }

    public bool? ResetPasswordRequired { get; set; } = false;

    public string? ResetPasswordCode { get; set; }

    public DateTime? ResetPasswordSentAt { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool? IsDelete { get; set; } = false;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductFavorite> ProductFavorites { get; set; } = new List<ProductFavorite>();

    public virtual ICollection<ProductReviewInteraction> ProductInteractions { get; set; } = new List<ProductReviewInteraction>();

    public virtual ICollection<ProductReport> ProductReports { get; set; } = new List<ProductReport>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<ReportProductReview> ReportProductReviews { get; set; } = new List<ReportProductReview>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}
