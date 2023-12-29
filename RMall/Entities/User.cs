﻿using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int Status { get; set; }

    public int Level { get; set; }

    public decimal Money { get; set; }

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiry { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<UserPromotion> UserPromotions { get; set; } = new List<UserPromotion>();
}
