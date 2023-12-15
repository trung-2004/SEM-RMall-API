using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Brand
{
    public int Id { get; set; }

    public string Image { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int ShopId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<BrandIntroduction> BrandIntroductions { get; set; } = new List<BrandIntroduction>();

    public virtual Shop Shop { get; set; } = null!;
}
