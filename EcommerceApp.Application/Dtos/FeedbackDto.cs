namespace EcommerceApp.Application.Dtos
{
    public class FeedbackDto
    {
        public required string Comment { get; set; }
        public required int Rating { get; set; }
    }
    public class GetFeedbackDto
    {
        public required string Comment { get; set; }
        public required int Rating { get; set; }
        public required DateTime GivenAt { get; set; }
    }
}
