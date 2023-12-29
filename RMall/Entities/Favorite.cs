using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Favorite
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
