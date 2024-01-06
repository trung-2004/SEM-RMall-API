using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Shops
{
    public class CreateShop
    {
        [Required(ErrorMessage = "Please enter title")]
        [MinLength(3, ErrorMessage = "Enter at least 3 characters")]
        [MaxLength(255, ErrorMessage = "Enter up to 255 characters")]
        public string name { get; set; }

        [Required(ErrorMessage = "Please enter image")]
        public IFormFile imagePath { get; set; }

        [Required(ErrorMessage = "Please enter floor")]
        public int floorId { get; set; }

        [Required(ErrorMessage = "Please enter category")]
        public int categoryId { get; set; }

        public string? contactInfo { get; set; }

        public string? hoursOfOperation { get; set; }

        public string? description { get; set; }

        public string? address { get; set; }
    }
}
