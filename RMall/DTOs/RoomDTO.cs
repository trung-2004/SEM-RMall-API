namespace RMall.DTOs
{
    public class RoomDTO : AbstractDTO<RoomDTO>
    {
        public string name { get; set; }

        public string slug { get; set; }

        public int rows { get; set; }

        public int columns { get; set; }
    }
}
