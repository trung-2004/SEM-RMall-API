namespace RMall.DTOs
{
    public class CategoryDTO : AbstractDTO<CategoryDTO>
    {
        public string name { get; set; }

        public string slug { get; set; }
    }
}
