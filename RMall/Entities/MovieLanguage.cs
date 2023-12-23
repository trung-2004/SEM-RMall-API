using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class MovieLanguage
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int LanguageId { get; set; }

    public virtual Language Language { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
