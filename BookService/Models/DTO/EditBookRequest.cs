namespace BookService.Models.DTO
{
    public class EditBookRequest
    {
        public string Title { get; set; }

        public string? Auther { get; set; }

        public string? Category { get; set; }

        public byte[]? Image { get; set; }

        public double? Price { get; set; }

        public string? Publisher { get; set; }

        public bool? Active { get; set; }

        public string? Content { get; set; }
    }
}
