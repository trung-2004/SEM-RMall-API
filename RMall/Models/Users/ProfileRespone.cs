namespace RMall.Models.Users
{
    public class ProfileRespone
    {
        public int id { get; set; }

        public string fullname { get; set; } = null!;

        public DateTime? birthday { get; set; }

        public string email { get; set; } = null!;

        public string? phone { get; set; }
    }
}
