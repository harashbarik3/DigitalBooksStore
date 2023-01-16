//using Microsoft.CodeAnalysis.Operations;

namespace DigitalBookStore.Web.Models
{
    public class Book
    {
        public Guid? Id { get; set; }
        public string?  Bookname { get; set; }
        public string?  Bookcontent { get; set; }
        public decimal? Price { get; set; }
        public string?  Categoryname { get; set; }
        public string? Publishername { get; set; }
        public string? Publisheddate { get; set; }
        public string? token { get; set; }
        public string? loggeduserId { get; set; }

        public bool? IsBlocked { get; set; }

        public byte[] Image { get; set; }

        public string? Auther { get; set; }
    }
}
