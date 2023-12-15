using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Users
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress]
        public string email { get; set; }
    }
}
