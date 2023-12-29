using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class OrderFood
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int FoodId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
