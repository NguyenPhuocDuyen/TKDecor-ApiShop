using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Product : BaseEntity
{
    public Guid ProductId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? Product3DModelId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Product3DModel? Product3DModel { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductFavorite> ProductFavorites { get; set; } = new List<ProductFavorite>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductReport> ProductReports { get; set; } = new List<ProductReport>();
}
