namespace RMall.Models.Seats
{
    public class SeatResponse
    {
        public int id { get; set; }

        public int roomId { get; set; }

        public int seatTypeId { get; set; }

        public int rowNumber { get; set; }

        public int seatNumber { get; set; }

        public bool isBooked { get; set; }

        public bool isReserved { get; set; }

        public decimal price { get; set; }

        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? deletedAt { get; set; }
    }
}
