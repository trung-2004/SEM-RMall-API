using RMall.Models.FoodOrder;
using RMall.Models.Tickets;

namespace RMall.Models.Orders
{
    public class OrderDetail
    {
        public int id { get; set; }
        public string orderCode { get; set; }

        public int showId { get; set; }

        public string movieName { get; set; }

        public int userId { get; set; }

        public string userName { get; set; }

        public decimal total { get; set; }

        public decimal discountAmount { get; set; }

        public string? discountCode { get; set; }

        public decimal finalTotal { get; set; }

        public int status { get; set; }

        public string paymentMethod { get; set; }

        public int isPaid { get; set; }

        public string qrCode { get; set; }

        public List<TicketResponse> tickets { get; set; }

        public List<OrderFoodResponse> foods { get; set; }
        public DateTime? createdAt { get; set; }

        public DateTime? updatedAt { get; set; }

        public DateTime? deletedAt { get; set; }

    }
}
