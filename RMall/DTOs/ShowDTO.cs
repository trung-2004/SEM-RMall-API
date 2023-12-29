using RMall.Models.MovieGenre;
using RMall.Models.SeatPricings;

namespace RMall.DTOs
{
    public class ShowDTO : AbstractDTO<ShowDTO>
    {
        public string showCode { get; set; }

        public int movieId { get; set; }

        public string? movieName { get; set; }

        public int roomId { get; set; }

        public string? roomName { get; set; }

        public DateTime startDate { get; set; }

        public string language { get; set; }
        public List<SeatPricingResponse>? seatPricings { get; set; }

    }
}
