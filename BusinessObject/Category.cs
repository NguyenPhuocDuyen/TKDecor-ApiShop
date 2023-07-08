namespace BusinessObject;

public partial class Category : BaseEntity
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
