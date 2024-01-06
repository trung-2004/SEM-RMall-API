using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Floor
{
    public int Id { get; set; }

    public string FloorNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
