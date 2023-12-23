using System;
using System.Collections.Generic;

namespace RMall.Entities;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Actor { get; set; } = null!;

    public string MovieImage { get; set; } = null!;

    public string CoverImage { get; set; } = null!;

    public string? Describe { get; set; }

    public string Director { get; set; } = null!;

    public int Duration { get; set; }

    public string Ratings { get; set; } = null!;

    public string? Trailer { get; set; }

    public string Cast { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Gallery> Galleries { get; set; } = new List<Gallery>();

    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    public virtual ICollection<MovieLanguage> MovieLanguages { get; set; } = new List<MovieLanguage>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
