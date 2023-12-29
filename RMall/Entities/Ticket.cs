using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Ticket
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int OrderId { get; set; }

    public DateTime StartDate { get; set; }

    public int SeatId { get; set; }

    public decimal Price { get; set; }

    public int IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;
}
