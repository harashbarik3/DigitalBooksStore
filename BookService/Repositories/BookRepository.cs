using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CommonLib.Models;
using UserServiceLib;
using System;

using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.JsonPatch;

namespace BookService.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly DigitalbookstoreContext _context;

        public BookRepository(DigitalbookstoreContext context)
        {
            _context = context;

        }

        public BookRepository()
        {
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            book.BookId = Guid.NewGuid();            
            await _context.AddAsync(book);
            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<IEnumerable<Book>> GetAllBookAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<IEnumerable<Book>> Search(string BookName, double? price, string? categoryName, string? publisherName)
        {
            IQueryable<Book> query = _context.Books;
            IQueryable<Category> category = _context.Categories;
            IQueryable<Publisher> publishers = _context.Publishers;

            if (!string.IsNullOrEmpty(BookName))
            {
                query = query.Where(x => x.BookName.ToLower().Contains(BookName.ToLower()));
            }

            if (publisherName != null)
            {
                query = from book in query
                        join publisher in publishers
                        on book.PublisherId equals publisher.PublisherId
                        select book;
            }

            if (category != null)
            {
                query = from book in query
                        join c in category
                        on book.CategoryId equals c.CategoryId
                        select book;
            }


            if (price != 0 || price != null)
            {
                query = query.Where(x => x.Price == (decimal)price);
            }

            else
            {
                return null;
            }

            return await query.ToListAsync();
        }

        public async Task<Book> UpdateBook(Book book, Guid id)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(x => x.BookId == id);

            if (existingBook != null)
            {
                existingBook.BookName = book.BookName;
                existingBook.CategoryId = book.CategoryId;
                existingBook.Active = book.Active;
                existingBook.Price = book.Price;
                existingBook.PublisherId = book.PublisherId;
                existingBook.PublishedDate = book.PublishedDate;
                existingBook.BookContent = book.BookContent;
                existingBook.UserId = book.UserId;

                await _context.SaveChangesAsync();
                return existingBook;
            }
            else
            {
                return null;
            }
        }

        public async Task<Book> DeleteBookAsync(Guid id)
        {
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(x => x.BookId == id);

                if (book != null)
                {
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                    return book;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Book> GetBookById(Guid id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                return book;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Book>> GetAllSubscribedBooksAsync(string email)
        {
            try
            {
                IQueryable<Book> _books = _context.Books;
                IQueryable<Subscription> _subscription = _context.Subscriptions;
                IQueryable<User> _users = _context.Users;

                var userids = _users.Where(x => x.Email == email).FirstOrDefault().UserId;

                _books = from book in _books
                         join s in _subscription
                         on book.BookId equals s.BookId
                         where s.UserId == userids
                         select book;

                //var userid = from user in _users
                //             join s in _subscription on user.UserId equals s.UserId
                //             where user.Email.ToLower() == email.ToLower()
                //             select s.UserId;

                //var bookres = _books.Where(x => BookId.Contains(x.BookId)).ToListAsync();


                return await _books.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;

            }
        }
        public async Task<IEnumerable<Book>> GetSubscribedBook(string email, Guid subscriptionId)
        {
            try
            {
                IQueryable<Book> _books = _context.Books;
                IQueryable<Subscription> _subscription = _context.Subscriptions;
                IQueryable<User> _users = _context.Users;

                _users = from user in _users
                         join subscription in _subscription
                        on user.UserId equals subscription.UserId
                         where user.Email.ToLower() == email.ToLower()
                         && subscription.SubscriptionId == subscriptionId
                         select user;

                _books = from book in _books
                         join user in _users
                         on book.UserId equals user.UserId
                         select book;

                return await _books.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetSubscribedBookContent(string email, Guid subscriptionId)
        {
            try
            {
                var _books = _context.Books;
                var _users = await _context.Users.Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
                var _subscription = await _context.Subscriptions.Where(x => x.SubscriptionId == subscriptionId && x.UserId == _users.UserId).FirstOrDefaultAsync();
                var book = await _books.Where(x => x.BookId == _subscription.BookId).FirstOrDefaultAsync();
                if (book != null)
                {
                    return book.BookContent;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task BlockBookAsync(Guid bookId,JsonPatchDocument bookModal,Guid autherId)
        {
            try
            {
                var book = await _context.Books.Where(x => x.BookId == bookId && x.UserId == autherId).FirstOrDefaultAsync();

                if (book != null)
                {
                    bookModal.ApplyTo(book);
                    await _context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                
            }
            

        }

        public async Task UnBlockBookAsync(Guid bookId, JsonPatchDocument bookModal, Guid autherId)
        {
            try
            {
                var book = await _context.Books.Where(x => x.BookId == bookId && x.UserId == autherId).FirstOrDefaultAsync();

                if (book != null)
                {
                    bookModal.ApplyTo(book);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }


        }

    }
        
}