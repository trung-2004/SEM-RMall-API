namespace RMall.Models.Tickets
{
    public class TicketResponse
    {
        public int id { get; set; }
        public string code { get; set; }

        public int orderId { get; set; }

        public DateTime startDate { get; set; }

        public int seatId { get; set; }

        public string seatName { get; set; }

        public decimal price { get; set; }

        public int isUsed { get; set; }
    }
}
