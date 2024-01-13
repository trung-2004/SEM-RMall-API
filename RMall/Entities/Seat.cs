using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Seat
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public int SeatTypeId { get; set; }

    public int RowNumber { get; set; }

    public int SeatNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<SeatReservation> SeatReservations { get; set; } = new List<SeatReservation>();

    public virtual SeatType SeatType { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
