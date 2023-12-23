using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Floor
{
    public int Id { get; set; }

    public int FloorNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
}
