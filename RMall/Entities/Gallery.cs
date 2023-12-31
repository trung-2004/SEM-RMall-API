﻿using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Gallery
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public string ImagePath { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Movie Movie { get; set; } = null!;
}
