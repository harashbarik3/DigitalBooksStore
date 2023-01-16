using CommonLib.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BookService.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBookAsync();
        Task<Book> GetBookById(Guid id);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBook(Book book, Guid id);
        Task<IEnumerable<Book>> Search(string BookName, double? price, string? categoryName, string? publisherName);
        Task<Book> DeleteBookAsync(Guid id);
        Task<IEnumerable<Book>> GetAllSubscribedBooksAsync(string email);
        Task<IEnumerable<Book>> GetSubscribedBook(string email, Guid subscriptionId);
        Task<string> GetSubscribedBookContent(string email, Guid subscriptionId);
        Task BlockBookAsync(Guid bookId, JsonPatchDocument bookModal1, Guid autherId);
        Task UnBlockBookAsync(Guid bookId, JsonPatchDocument bookModal, Guid autherId);
    }
}
