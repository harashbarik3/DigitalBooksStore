namespace BookService.Models.DTO
{
    public class AddBookRequest
    {
        public string BookName { get; set; }

        public string? Auther { get; set; }

        public string? Category { get; set; }        

        public double? Price { get; set; }

        public string? Publisher { get; set; }

        public bool? Active { get; set; }

        public string? Content { get; set; }
    }
}
