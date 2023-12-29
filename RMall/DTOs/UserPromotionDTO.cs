namespace RMall.DTOs
{
    public class UserPromotionDTO : AbstractDTO<UserPromotionDTO>
    {
        public int userId { get; set; }

        public string? nameUser { get; set; }

        public int promotionId { get; set; }

        public string? promotionName { get; set; }

        public string? promotionCode { get; set; }

        public int isUsed { get; set; }

        public DateTime? usedAt { get; set; }
    }
}
