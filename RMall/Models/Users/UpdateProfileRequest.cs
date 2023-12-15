using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Users
{
    public class UpdateProfileRequest
    {
        [Required]
        public int id { get; set; }

        [Required]
        public DateTime birthday { get; set; }

        public string? phone { get; set; }
    }
}
