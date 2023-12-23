namespace RMall.Models.SeatPricings
{
    public class SeatPricingResponse
    {
        public int id { get; set; }

        public int showId { get; set; }

        public string seatTypeName { get; set; }

        public int seatTypeId { get; set; }

        public decimal price { get; set; }
    }
}
