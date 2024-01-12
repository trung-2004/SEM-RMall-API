namespace RMall.DTOs
{
    public class ProductDTO : AbstractDTO<ProductDTO>
    {
        public string name { get; set; }

        public string image { get; set; }

        public decimal? price { get; set; }

        public string? description { get; set; }

        public int shopId { get; set; }

        public string? shopName { get; set; }
    }
}
