using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class SeatPricing
{
    public int Id { get; set; }

    public int ShowId { get; set; }

    public int SeatTypeId { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual SeatType SeatType { get; set; } = null!;

    public virtual Show Show { get; set; } = null!;
}
