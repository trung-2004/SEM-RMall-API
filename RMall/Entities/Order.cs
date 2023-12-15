using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Order
{
    public int Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public int ShowId { get; set; }

    public int UserId { get; set; }

    public decimal Total { get; set; }

    public decimal DiscountAmount { get; set; }

    public string? DiscountCode { get; set; }

    public decimal FinalTotal { get; set; }

    public int Status { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public int IsPaid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<OrderFood> OrderFoods { get; set; } = new List<OrderFood>();

    public virtual Show Show { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User User { get; set; } = null!;
}
