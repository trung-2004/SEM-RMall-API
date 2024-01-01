namespace RMall.DTOs
{
    public class FavoriteDTO : AbstractDTO<FavoriteDTO>
    {
        public int movieId { get; set; }

        public string movieName { get; set; }

        public int userId { get; set; }
    }
}
