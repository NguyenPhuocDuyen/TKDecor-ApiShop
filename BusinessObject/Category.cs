using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
