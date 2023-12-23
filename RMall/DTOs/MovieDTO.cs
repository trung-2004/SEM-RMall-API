using RMall.Entities;
using RMall.Models.Galleries;
using RMall.Models.MovieGenre;
using RMall.Models.MovieLanguage;

namespace RMall.DTOs
{
    public class MovieDTO : AbstractDTO<MovieDTO>
    {
        public string title { get; set; }

        public string actor { get; set; }

        public string movie_image { get; set; }

        public string? describe { get; set; }

        public string director { get; set; }

        public int duration { get; set; }

        public string ratings { get; set; }

        public string? trailer { get; set; }

        public string cast { get; set; }

        public DateTime release_date { get; set; }

        public List<GenreMovieResponse>? genres { get; set; }

        public List<LanguageMovieResponse>? languages { get; set; }

        public List<GaleryResponse>? galeries { get; set; }
    }
}
