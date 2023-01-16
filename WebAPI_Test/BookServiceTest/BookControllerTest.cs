using BookService.Controllers;
using BookService.Repositories;
using CommonLib.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI_Test.BookServiceTest
{
    public class BookControllerTest
    {
        private readonly BooksController _controller;
        private readonly IBookRepository _bookRepository;

        public BookControllerTest()
        {
            _bookRepository = new BookTest();
            _controller = new BooksController(_bookRepository);
        }

        [Fact]
        public async void Get_WhenCalled_ReturnsOkResult()
        {
            // ACT
            var okResult = await _controller.GetAllBooks();           

            //Assert
             Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }

        [Fact]
        public async void Get_WhenCalled_ReturnAllMethods()
        {
            //Act
            var okResult = await _controller.GetAllBooks() as OkObjectResult;

            //Asert
            var items = Assert.IsType<List<Book>>(okResult.Value);
             Assert.Equal(2, items.Count);
        }

        [Fact]
        public async void GetById_UnKnownGuidPassed_ReturnNotFoundResult()
        {
            //Act
            var notFoundResult = await _controller.GetBookById(new Guid());

            //Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
        }

        [Fact]
        public async void GetById_ExistingGuidPassed_ReturnOkResutl()
        {
            var testGuid = new Guid("2160FD1F-000B-4FFF-A4F3-921A7ECD1A51");

            //Act
            var okResult = await _controller.GetBookById(testGuid);

            //Assert
            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }

        [Fact]

        public async void Add_ValidObjectPassed_ReturnsCreatedResponse()
        {
            Book book = new Book()
            {
                BookId = new Guid(),
                BookName = "Spiderman 2",
                CategoryId = new Guid("7F8CD994-9C2D-4CC7-871A-E3CF7EB43322"),
                BookContent = "The Spiderman 2nd version",
                Price = 4500,
                Active = true,
                PublishedDate = "2022-12-28 23:27:33.1787982",
                PublisherId = new Guid("92E2FB96-2FFC-4526-8145-59847DFA0C32"),
                UserId = new Guid("92B79EC0-186D-440A-81DA-0A76170E832B"),
            };

            //Act
            var CreatedAtResponse = await _controller.CreateBook(book,book.UserId);

            //Assert

            Assert.IsType<CreatedAtActionResult>(CreatedAtResponse);
        }

        [Fact]
        public async void Get_WhenCalled_ReturnAllSubscribedMethods()
        {
            var email = "user2@gmail.com";
            //Act
            var okResult = await _controller.GetAllSubscribedBooks(email) as OkObjectResult;

            //Asert
            var items = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(1, items.Count);
        }

        [Fact]
        public async void Get_WhenCalled_ReturnAllSubscribedBookMethods()
        {
            var email = "user2@gmail.com";
            var subscriptionId = new Guid("565586B8-82B3-487C-ADB5-C62E9DD49284");
            //Act
            var okResult = await _controller.GetSubscribedBook(email,subscriptionId) as OkObjectResult;

            //Asert
            var items = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(1, items.Count);
        }

        [Fact]
        public async void Get_WhenCalled_ReturnSubscribedBookContent()
        {
            var email = "user2@gmail.com";
            var subscriptionId = new Guid("565586B8-82B3-487C-ADB5-C62E9DD49284");
            var bookContent = "The Batman story";

            var bookContentResult = await _controller.GetSubscribedBookContent(email, subscriptionId) as OkObjectResult;

            var item=Assert.IsType<string>(bookContentResult.Value);

            Assert.Equal(bookContent, item);            
        }

        [Fact]
        public async void Update_WhenValidBookidPassed()
        {
            var booId = new Guid("2160FD1F-000B-4FFF-A4F3-921A7ECD1A51");
            var autherId = new Guid("92B79EC0-186D-440A-81DA-0A76170E832B");
            Book book = new Book()
            {
                BookId = new Guid(),
                BookName = "Spiderman 2",
                CategoryId = new Guid("7F8CD994-9C2D-4CC7-871A-E3CF7EB43322"),
                BookContent = "The Spiderman 2nd version",
                Price = 4500,
                Active = true,
                PublishedDate = "2022-12-28 23:27:33.1787982",
                PublisherId = new Guid("92E2FB96-2FFC-4526-8145-59847DFA0C32"),
                UserId = autherId
            };

            //Act
            var okObjectResult = await _controller.EditBook(book, autherId, booId) as OkObjectResult;

            //Assert

            Assert.IsType<OkObjectResult>(okObjectResult);

        }

        [Fact]
        public async void Get_WhenSearchBook()
        {
            var bookName = "BatMan";
            double price = 3000;
            string categoryName = "Comic";
            string publisherName = "DC";

            var okObjectResult = await _controller.SearchBook(categoryName,bookName,price,publisherName) as OkObjectResult;

            var item = Assert.IsType<OkObjectResult>(okObjectResult);

            Assert.Equal(1, item.Count);

        }


    }  
}
