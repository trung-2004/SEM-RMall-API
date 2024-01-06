using RMall.Models.Products;

namespace RMall.Models.Shops
{
    public class ShopDetail
    {
        public int id { get; set; }
        public string name { get; set; }

        public string imagePath { get; set; }

        public string slug { get; set; }

        public int floorId { get; set; }

        public string? floorName { get; set; }

        public int categoryId { get; set; }

        public string? categoryName { get; set; }

        public string? contactInfo { get; set; }

        public string? hoursOfOperation { get; set; }

        public string? description { get; set; }

        public string? address { get; set; }

        public List<ProductResponse>? products { get; set; }

        public DateTime? createdAt { get; set; }

        public DateTime? updatedAt { get; set; }

        public DateTime? deletedAt { get; set; }
    }
}
