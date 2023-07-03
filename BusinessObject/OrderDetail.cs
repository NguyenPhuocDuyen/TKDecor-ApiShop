using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class OrderDetail
{
    public long OrderDetailId { get; set; }

    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal PaymentPrice { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
