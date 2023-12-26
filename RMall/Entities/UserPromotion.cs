using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class UserPromotion
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PromotionId { get; set; }

    public int IsUsed { get; set; }

    public DateTime? UsedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Promotion Promotion { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
