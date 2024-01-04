using RMall.Models.SeatPricings;
using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Shows
{
    public class CreateShow
    {
        [Required(ErrorMessage = "Please enter movie")]
        public int movieId { get; set; }

        [Required(ErrorMessage = "Please enter room")]
        public int roomId { get; set; }

        [Required(ErrorMessage = "Please enter start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime startDate { get; set; }

        [Required(ErrorMessage = "Please enter language")]
        public string language { get; set; }

        [Required(ErrorMessage = "Please enter seat pricing")] 
        public List<CreateSeatPricing> seatPricings { get; set; }
    }
}
