using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class SeatReservation
{
    public int Id { get; set; }

    public int SeatId { get; set; }

    public int ShowId { get; set; }

    public DateTime? ReservationExpiresAt { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    public virtual Show Show { get; set; } = null!;
}
