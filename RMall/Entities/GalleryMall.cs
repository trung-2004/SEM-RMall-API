using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class GalleryMall
{
    public int Id { get; set; }

    public string ImagePath { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
