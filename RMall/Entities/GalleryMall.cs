﻿using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class GalleryMall
{
    public int Id { get; set; }

    public string ImagePath { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
