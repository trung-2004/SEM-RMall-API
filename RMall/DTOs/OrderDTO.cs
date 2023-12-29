namespace RMall.DTOs
{
    public class OrderDTO : AbstractDTO<OrderDTO>
    {
        public string orderCode { get; set; }

        public int showId { get; set; }

        public int userId { get; set; }

        public decimal total { get; set; }

        public decimal discountAmount { get; set; }

        public string? discountCode { get; set; }

        public decimal finalTotal { get; set; }

        public int status { get; set; }

        public string paymentMethod { get; set; }

        public int isPaid { get; set; }
    }
}
