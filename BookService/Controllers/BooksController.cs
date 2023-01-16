using AutoMapper;
using BookService.Models.DTO;
using BookService.Repositories;
using CommonLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookService.Controllers
{
    [Route("api/v1/digitalbooks")]
    [ApiController]    
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;        
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;            
        }

        [HttpGet("")]
        
                
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookRepository.GetAllBookAsync();            
            return Ok(books);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookById([FromRoute] Guid id)
        {
            try
            {
                var book = await _bookRepository.GetBookById(id);
                if(book==null)
                {
                    throw new Exception();
                }
                return Ok(book);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            
        }

        [HttpPost("auther/{auther_id:guid}/books")]
        [Authorize(Roles = "AUTHER")]
        public async Task<IActionResult> CreateBook([FromBody] Book book, [FromRoute]Guid auther_id)
        {
            try
            {
                var addBook = new Book()
                {
                    BookName = book.BookName,
                    CategoryId = book.CategoryId,
                    Active = book.Active,
                    Price = book.Price,
                    PublisherId = book.PublisherId,
                    PublishedDate = book.PublishedDate,
                    BookContent = book.BookContent,
                    UserId = auther_id,
                    Image=book.Image,
                    CategoryName= book.CategoryName,
                    PublisherName= book.PublisherName

                };

                var bookres = await _bookRepository.AddBookAsync(addBook);
                return CreatedAtAction(nameof(GetBookById), new { id = bookres.BookId, controller = "books" }, bookres);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("auther/{auther_id}/books/{book_id:guid}")]
        [Authorize(Roles = "AUTHER")]
        public async Task<IActionResult> EditBook([FromBody]Book book, [FromRoute] Guid auther_id, [FromRoute]Guid book_id)
        {
            try
            {
                var editBook = new Book()
                {
                    BookId= book_id,
                    BookName=book.BookName,
                    CategoryId=book.CategoryId,
                    Active = book.Active,
                    Price = book.Price,
                    PublisherId = book.PublisherId,
                    PublishedDate = book.PublishedDate,
                    BookContent = book.BookContent,
                    UserId= auther_id,
                    Image=book.Image,
                    CategoryName= book.CategoryName,
                    PublisherName= book.PublisherName
                   
                };
                var bookRes = await _bookRepository.UpdateBook(editBook, book_id);

                return Ok(bookRes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]        
        public async Task<IActionResult> SearchBook(string? categoryName, string bookName, double? price, string? publisherName)
        {
            try
            {
                var book = await _bookRepository.Search(bookName,price, categoryName, publisherName);               

                if (book != null)
                {
                    return Ok(book);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }

        [HttpDelete("delete/book/{id:guid}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                var book = await _bookRepository.GetBookById(id); ;

                if (book != null)
                {

                    await _bookRepository.DeleteBookAsync(book.BookId);
                    return Ok(book);
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }            

        [HttpGet("reader/{emailId}/book")]
        [Authorize(Roles = "READER")]
        public async Task<IActionResult> GetAllSubscribedBooks(string emailId)
        {
            try
            {                
                var books = await _bookRepository.GetAllSubscribedBooksAsync(emailId);

                if(books != null)
                {
                    return Ok(books);
                }
                else
                {
                    throw new Exception();
                }

            }
            catch(Exception ex)
            {
                return NotFound($"For {emailId} there is no subscribed book");
            }   

        }

        [HttpGet("reader/{emailid}/books/{subscription_id:guid}")]
        [Authorize(Roles = "READER")]
        public async Task<IActionResult> GetSubscribedBook(string emailid, Guid subscription_id)
        {
            try
            {
                var subscribedBook = await _bookRepository.GetSubscribedBook(emailid, subscription_id);

                if (!subscribedBook.IsNullOrEmpty())
                {
                    return Ok(subscribedBook);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch(Exception ex)
            {
                return BadRequest($"The Entered Email {emailid} and subscriptionid {subscription_id} is not found");
            }       
        }

        [HttpGet("readers/{emailid}/books/{subscription_id}/read")]
        //[Authorize(Roles = "READER")]
        public async Task<IActionResult> GetSubscribedBookContent(string emailid, Guid subscription_id)
        {
            try
            {
                var bookContent = await _bookRepository.GetSubscribedBookContent(emailid, subscription_id);

                if (bookContent != null)
                {
                    return Ok(bookContent);
                }
                else
                {
                    throw new Exception();
                }

            }
            catch(Exception ex)
            {
                return BadRequest();
            }
           
        }

        [HttpPatch("auther/{auther_id}/book/block/{book_id}")]
        public async  Task<IActionResult> BlockBookById([FromBody]JsonPatchDocument book, [FromRoute]Guid book_id, [FromRoute]Guid auther_id)
        {
            await _bookRepository.BlockBookAsync(book_id, book,auther_id);
            return Ok();
        }

        [HttpPatch("auther/{auther_id}/book/unblock/{book_id}")]
        public async Task<IActionResult> UnBlockBookById([FromBody] JsonPatchDocument book, [FromRoute] Guid book_id, [FromRoute] Guid auther_id)
        {
            try
            {
                await _bookRepository.BlockBookAsync(book_id, book, auther_id);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
            
        }


    }
}
