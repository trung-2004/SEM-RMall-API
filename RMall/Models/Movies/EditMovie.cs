using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Movies
{
    public class EditMovie
    {
        [Required]
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter title")]
        [MinLength(3, ErrorMessage = "Enter at least 3 characters")]
        [MaxLength(255, ErrorMessage = "Enter up to 255 characters")]
        public string title { get; set; }

        [Required(ErrorMessage = "Please enter actor")]
        public string actor { get; set; }

        [Required(ErrorMessage = "Please enter movie image")]
        public IFormFile movie_image { get; set; }
   
        [Required(ErrorMessage = "Please enter cover image")]
        public IFormFile cover_image { get; set; }

        public string? describe { get; set; }

        [Required(ErrorMessage = "Please enter mirector")]
        public string director { get; set; }

        [Required(ErrorMessage = "Please enter duration")]
        public int duration { get; set; }

        public string? trailer { get; set; }

        [Required(ErrorMessage = "Please enter release date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime release_date { get; set; }
    }
}
