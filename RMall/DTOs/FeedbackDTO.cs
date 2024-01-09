namespace RMall.DTOs
{
    public class FeedbackDTO : AbstractDTO<FeedbackDTO>
    {
        public string name { get; set; } 

        public string email { get; set; } 

        public string phone { get; set; } 

        public string message { get; set; }
    }
}
