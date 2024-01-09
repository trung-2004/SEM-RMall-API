namespace RMall.DTOs
{
    public class GalleryDTO : AbstractDTO<GalleryDTO>
    {
        public string imagePath { get; set; }

        public string productName { get; set; }

        public string? description { get; set; }
    }
}
