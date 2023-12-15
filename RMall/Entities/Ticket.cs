using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Ticket
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public DateTime StartDate { get; set; }

    public int RowNum { get; set; }

    public int SeatNum { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
