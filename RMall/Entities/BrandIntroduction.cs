using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class BrandIntroduction
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int BrandId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Brand Brand { get; set; } = null!;
}
