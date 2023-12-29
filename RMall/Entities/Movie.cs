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

    public int FavoriteCount { get; set; }

    public string? Trailer { get; set; }

    public DateTime ReleaseDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<GalleryMovie> GalleryMovies { get; set; } = new List<GalleryMovie>();

    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    public virtual ICollection<MovieLanguage> MovieLanguages { get; set; } = new List<MovieLanguage>();

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
