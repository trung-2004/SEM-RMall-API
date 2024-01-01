using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Favorites
{
    public class CreateFavorite
    {
        [Required]
        public int movieId { get; set; }
    }
}
