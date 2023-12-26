namespace RMall.DTOs
{
    public class FoodDTO : AbstractDTO<FoodDTO>
    {
        public string name { get; set; }

        public decimal price { get; set; }

        public int quantity { get; set; }
    }
}
