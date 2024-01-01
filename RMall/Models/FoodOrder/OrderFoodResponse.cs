namespace RMall.Models.FoodOrder
{
    public class OrderFoodResponse
    {
        public int id { get; set; }

        public int orderId { get; set; }

        public int foodId { get; set; }

        public string foodName { get; set; }

        public string foodImage { get; set; }

        public decimal price { get; set; }

        public int quantity { get; set; }
    }
}
