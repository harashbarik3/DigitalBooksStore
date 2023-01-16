using BookService.Repositories;
using CommonLib.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace WebAPI_Test.BookServiceTest
{
    internal class BookTest : IBookRepository
    {
        private readonly List<Book> _books;
        private readonly List<Category> _categories;
        private readonly List<Publisher> _publishers;
        private readonly List<User> _users;
        private readonly List<Subscription> _subscriptions;

        public BookTest()
        {
            _books = new List<Book>()
            {
                new Book(){
                    BookId=new Guid("2160FD1F-000B-4FFF-A4F3-921A7ECD1A51"),BookName="Spiderman",CategoryId=new Guid("7F8CD994-9C2D-4CC7-871A-E3CF7EB43322"),BookContent="The Spiderman book",Price=1200,Active=true,PublishedDate="2022-12-28 23:27:33.1787982",PublisherId=new Guid("92E2FB96-2FFC-4526-8145-59847DFA0C32"),UserId=new Guid("92B79EC0-186D-440A-81DA-0A76170E832B"),
                },
                 new Book(){
                    BookId=new Guid("9464DAEE-727D-46B5-9567-19E0FEA84C22"),BookName="BatMan",CategoryId=new Guid("66C42B33-17AB-4F30-B72A-B3B8B116C5D6"),BookContent="The Batman story",Price=3000,Active=true,PublishedDate="2022-12-28 23:27:33.1787982",PublisherId=new Guid("928454CE-C488-4A58-BDBA-D3391192FCF1"),UserId=new Guid("1369FF5C-F635-434B-88EB-63C6663CD159"),
                }

            };

            _categories = new List<Category>()
            {
                new Category(){ CategoryId=new Guid("7F8CD994-9C2D-4CC7-871A-E3CF7EB43322"),CategoryName="Story"},
                new Category(){ CategoryId=new Guid("66C42B33-17AB-4F30-B72A-B3B8B116C5D6"),CategoryName="Comic"}
            };

            _publishers = new List<Publisher>()
            {
                new Publisher(){PublisherId=new Guid("92E2FB96-2FFC-4526-8145-59847DFA0C32"),PublisherName="Marvel"},
                new Publisher(){PublisherId=new Guid("928454CE-C488-4A58-BDBA-D3391192FCF1"),PublisherName="DC"}
            };
            _subscriptions = new List<Subscription>()
            {
                new Subscription(){ SubscriptionId=new Guid("565586B8-82B3-487C-ADB5-C62E9DD49284"),BookId=new Guid("9464DAEE-727D-46B5-9567-19E0FEA84C22"),UserId=new Guid("1369FF5C-F635-434B-88EB-63C6663CD159")}
            };

            _users = new List<User>()
            {
                new User(){ UserId=new Guid("92B79EC0-186D-440A-81DA-0A76170E832B"),UserName="user1",Password="password123",RoleId=new Guid("CAD31216-98C5-4662-8D40-B66D4F3AF758"),Email="user1@gmail.com"},
                new User(){ UserId=new Guid("1369FF5C-F635-434B-88EB-63C6663CD159"),UserName="user2",Password="password@123",RoleId=new Guid("696E2CBD-4A86-47B6-91F9-E173C5CF9B61"),Email="user2@gmail.com"}
            };
        }
        public async Task<Book> AddBookAsync(Book book)
        {
            book.BookId = new Guid();
            _books.Add(book);
            return await Task.FromResult(book);
        }

        public Task BlockBookAsync(Guid bookId, JsonPatchDocument bookModal1, Guid autherId)
        {
            throw new NotImplementedException();
        }

        public async  Task<Book> DeleteBookAsync(Guid id)
        {
            var existing=_books.Where(a=>a.BookId==id).FirstOrDefault();

            if(existing!=null)
            {
                _books.Remove(existing);
            }

            return await Task.FromResult(existing);
            
        }

        public async  Task<IEnumerable<Book>> GetAllBookAsync()
        {
            return await Task.FromResult(_books);
        }

        
        public async Task<IEnumerable<Book>> GetAllSubscribedBooksAsync(string email)
        {
            try
            {
                List<Book> books =  _books;
                List<Subscription> subscription = _subscriptions;
                List<User> users = _users;

                var users1 = from user in users
                        join s in _subscriptions on user.UserId equals s.UserId
                        where user.Email.ToLower() == email.ToLower()
                        select user;

                var booksres = from book in books
                                          join user in users1
                                          on book.UserId equals user.UserId
                                          select book;

                return await Task.FromResult(booksres.ToList());
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        public async Task<Book> GetBookById(Guid id)
        {
            var book= _books.Where(x => x.BookId == id).FirstOrDefault();

            return await Task.FromResult(book);
        }

        public async Task<IEnumerable<Book>> GetSubscribedBook(string email, Guid subscriptionId)
        {
            try
            {
                List<Book> books = _books;
                List<Subscription> subscription = _subscriptions;
                List<User> users = _users;

               var  usersres = from user in users
                             join s in subscription
                             on user.UserId equals s.UserId
                             where user.Email.ToLower() == email.ToLower()
                             && s.SubscriptionId == subscriptionId
                             select user;

                var bookres = from book in books
                              join user in usersres
                              on book.UserId equals user.UserId
                              select book;

                return await Task.FromResult(bookres.ToList());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<string> GetSubscribedBookContent(string email, Guid subscriptionId)
        {
            try
            {
                var books = _books;
                var user =_users.Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();
                var subscription =  _subscriptions.Where(x => x.SubscriptionId == subscriptionId && x.UserId == user.UserId).FirstOrDefault();
                var book = books.Where(x => x.BookId == subscription.BookId).FirstOrDefault();
                if (book != null)
                {
                    return Task.FromResult(book.BookContent);
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

        public async Task<IEnumerable<Book>> Search(string BookName,double? price, string? categoryName, string? publisherName)
        {
            IQueryable<Book> query = _books.AsQueryable();
            IQueryable<Category> category = _categories.AsQueryable();
            IQueryable<Publisher> publishers = _publishers.AsQueryable();

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

        public Task UnBlockBookAsync(Guid bookId, JsonPatchDocument bookModal, Guid autherId)
        {
            throw new NotImplementedException();
        }

        public async Task<Book> UpdateBook(Book book, Guid id)
        {
            var existingBook = _books.Where(x=>x.BookId== id).FirstOrDefault();

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

                
                return await Task.FromResult(existingBook);
            }
            else
            {
                return null;
            }
        }
    }
}
