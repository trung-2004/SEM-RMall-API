using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int Rows { get; set; }

    public int Columns { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
