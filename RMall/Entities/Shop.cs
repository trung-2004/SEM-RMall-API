﻿using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Shop
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();
}
