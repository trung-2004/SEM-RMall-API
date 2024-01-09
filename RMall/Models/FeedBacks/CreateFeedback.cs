using System.ComponentModel.DataAnnotations;

namespace RMall.Models.FeedBacks
{
    public class CreateFeedback
    {
        [Required(ErrorMessage = "Please enter name")]
        [MinLength(3, ErrorMessage = "Enter at least 3 characters")]
        [MaxLength(255, ErrorMessage = "Enter up to 255 characters")]
        public string name { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression(@"^\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b", ErrorMessage = "Email address is not valid")]
        public string email { get; set; }

        [Required(ErrorMessage = "Please enter phone")]
        [MinLength(10, ErrorMessage = "Enter at least 10 characters")]
        [MaxLength(12, ErrorMessage = "Enter up to 12 characters")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Please enter message")]
        public string message { get; set; }
    }
}
