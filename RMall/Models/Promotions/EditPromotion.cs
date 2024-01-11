using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Promotions
{
    public class EditPromotion
    {
        [Required(ErrorMessage = "Please enter id")]
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [MinLength(3, ErrorMessage = "Enter at least 3 characters")]
        [MaxLength(255, ErrorMessage = "Enter up to 255 characters")]
        public string name { get; set; }

        [Required(ErrorMessage = "Please enter start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime startDate { get; set; }

        [Required(ErrorMessage = "Please enter end date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime endDate { get; set; }

        [Required(ErrorMessage = "Please enter discount percentage")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid decimal value.")]

        public int discountPercentage { get; set; }

        [Required(ErrorMessage = "Please enter limit")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid decimal value.")]
        public int limit { get; set; }

        [Required(ErrorMessage = "Please enter min puchase amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid decimal value.")]
        public decimal minPurchaseAmount { get; set; }
    }
}
