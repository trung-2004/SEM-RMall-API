using RMall.Models.Foods;
using RMall.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Orders
{
    public class CreateOrder
    {
        [Required(ErrorMessage = "Please enter showId")]
        public int showId { get; set; }

        [Required(ErrorMessage = "Please enter total")]
        public decimal total { get; set; }

        [Required(ErrorMessage = "Please enter discount amount")]
        public decimal discountAmount { get; set; }

        public string? discountCode { get; set; }

        [Required(ErrorMessage = "Please enter final total")]
        public decimal finalTotal { get; set; }

        [Required(ErrorMessage = "Please enter payment method")]
        public string paymentMethod { get; set; }

        [Required(ErrorMessage = "Please enter tickets")]
        public List<CreateTicketRequest> tickets { get; set; }

        public List<CreateFoodRequest>? foods { get; set; }
    }
}
