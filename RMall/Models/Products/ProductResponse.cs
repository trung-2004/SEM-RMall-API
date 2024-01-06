namespace RMall.Models.Products
{
    public class ProductResponse
    {
        public int id { get; set; }

        public string name { get; set; }

        public string image { get; set; }

        public decimal? price { get; set; }

        public string? description { get; set; }

    }
}
