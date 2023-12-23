using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Language
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<MovieLanguage> MovieLanguages { get; set; } = new List<MovieLanguage>();
}
