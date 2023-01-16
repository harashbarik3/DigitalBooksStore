namespace BookService.Models.DTO
{
    public class AddSubscriptionRequest
    {
        public string? userId { get; set; }
        public string? bookId { get; set; }
        string? subscriptionDate { get; set; }
    }
}
