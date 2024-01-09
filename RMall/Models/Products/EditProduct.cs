using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Products
{
    public class EditProduct
    {
        [Required(ErrorMessage = "Please enter id")]
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [MinLength(3, ErrorMessage = "Enter at least 3 characters")]
        [MaxLength(255, ErrorMessage = "Enter up to 255 characters")]
        public string name { get; set; }

        public IFormFile? image { get; set; }

        public decimal? price { get; set; }

        public string? description { get; set; }

        [Required(ErrorMessage = "Please enter shop")]
        public int shopId { get; set; }
    }
}
