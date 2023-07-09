namespace BusinessObject;

public partial class Cart : BaseEntity
{
    public Guid CartId { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
