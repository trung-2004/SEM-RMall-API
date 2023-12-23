using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Genre
{
    public class EditGenre
    {
        [Required]
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter title")]
        [MinLength(3, ErrorMessage = "Enter at least 3 characters")]
        [MaxLength(255, ErrorMessage = "Enter up to 255 characters")]
        public string name { get; set; }
    }
}
