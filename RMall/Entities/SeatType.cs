using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class SeatType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<SeatPricing> SeatPricings { get; set; } = new List<SeatPricing>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
