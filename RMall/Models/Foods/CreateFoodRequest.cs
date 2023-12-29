using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Foods
{
    public class CreateFoodRequest
    {
        [Required(ErrorMessage = "Please enter quantity")]
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid decimal value.")]
        public int quantity { get; set; }
    }
}
