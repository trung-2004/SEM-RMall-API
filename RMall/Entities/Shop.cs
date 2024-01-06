using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Shop
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ImagePath { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int FloorId { get; set; }

    public int CategoryId { get; set; }

    public string? ContactInfo { get; set; }

    public string? HoursOfOperation { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Floor Floor { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
