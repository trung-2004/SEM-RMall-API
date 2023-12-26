namespace RMall.DTOs
{
    public class PromotionDTO : AbstractDTO<PromotionDTO>
    {
        public string name { get; set; }

        public string slug { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public int discountPercentage { get; set; }

        public int limit { get; set; }

        public string couponCode { get; set; }

        public decimal minPurchaseAmount { get; set; }
    }
}
