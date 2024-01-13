using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Show
{
    public int Id { get; set; }

    public string ShowCode { get; set; } = null!;

    public int MovieId { get; set; }

    public int RoomId { get; set; }

    public DateTime StartDate { get; set; }

    public string Language { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<SeatPricing> SeatPricings { get; set; } = new List<SeatPricing>();

    public virtual ICollection<SeatReservation> SeatReservations { get; set; } = new List<SeatReservation>();
}
