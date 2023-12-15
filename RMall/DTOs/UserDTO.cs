namespace RMall.DTOs
{
    public class UserDTO : AbstractDTO<UserDTO>
    {
        public string fullname { get; set; } = null!;

        public DateTime? birthday { get; set; }

        public string email { get; set; } = null!;

        public string? phone { get; set; }

        public string password { get; set; } = null!;

        public string role { get; set; } = null!;

        public int status { get; set; }

        public string? resetToken { get; set; }

        public DateTime? resetTokenExpiry { get; set; } 
    }
}
