using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Promotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int DiscountPercentage { get; set; }

    public int Limit { get; set; }

    public string CouponCode { get; set; } = null!;

    public decimal MinPurchaseAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<UserPromotion> UserPromotions { get; set; } = new List<UserPromotion>();
}
