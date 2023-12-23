using System.ComponentModel.DataAnnotations;

namespace RMall.Models.SeatPricings
{
    public class CreateSeatPricing
    {
        [Required(ErrorMessage = "Please enter movie")]
        public int seatTypeId { get; set; }

        [Required(ErrorMessage = "Please enter movie")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid decimal value.")]
        public decimal price { get; set; }
    }
}
