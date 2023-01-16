using BookService.Repositories;
using CommonLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI_Test.BookServiceTest
{
    public class BookRepositoryTest
    {
        private readonly List<Book> _books;
        private readonly List<Category> _categories;
        private readonly List<Publisher> _publishers;

        public BookRepositoryTest()
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
        }
    }
}

        [Fact]
        //public async void Get_WhenCalled_ReturnAllMethods1()
        //{
        //    BookRepository repo= new BookRepository();

        //    var books = await repo.GetAllBookAsync();

        //    Assert.IsType<List<Book>>(books as List<Book>);
        //}
