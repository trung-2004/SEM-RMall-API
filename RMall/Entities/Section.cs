using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Section
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int FloorId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Floor Floor { get; set; } = null!;
}
