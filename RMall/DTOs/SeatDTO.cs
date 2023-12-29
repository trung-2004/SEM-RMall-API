namespace RMall.DTOs
{
    public class SeatDTO : AbstractDTO<SeatDTO>
    {
        public int roomId { get; set; }

        public int seatTypeId { get; set; }

        public int rowNumber { get; set; }

        public int seatNumber { get; set; }
    }
}
