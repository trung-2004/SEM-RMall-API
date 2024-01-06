using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public int ShopId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Shop Shop { get; set; } = null!;
}
