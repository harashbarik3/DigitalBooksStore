using CommonLib.Models;
using DigitalBookStore.Web.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
//using System.Text.Json.Serialization;


namespace DigitalBookStore.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpContextAccessor _context;
        public BookController(IHttpContextAccessor context)
        {
            _context = context;
        }

        string BaseUrl = "https://localhost:7128/";


        public async Task<IActionResult> Index(string sortOrder, string searchString, string? token)
        {
            try
            {
                ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "bookName_desc" : "";
                ViewData["BookContentSortParam"] = string.IsNullOrEmpty(sortOrder) ? "bookContent_desc" : "";
                ViewData["PriceSortParam"] = sortOrder == "Price" ? "Price_desc" : "Price";
                ViewData["PublisherSortParam"] = string.IsNullOrEmpty(sortOrder) ? "publisherName_desc" : "";
                ViewData["CategorySortParam"] = string.IsNullOrEmpty(sortOrder) ? "categoryName_desc" : "";
                ViewData["DateSortParam"] = string.IsNullOrEmpty(sortOrder) ? "publisheddate_desc" : "";

                ViewData["CurrentFilter"] = searchString;

                string tokenVal = _context.HttpContext.Session.GetString("token");

                string userIdVal = _context.HttpContext.Session.GetString("userId");

                string userRole = _context.HttpContext.Session.GetString("userRole");


                using (var client = new HttpClient())
                {
                    List<CommonLib.Models.Book> books = new List<CommonLib.Models.Book>();
                    List<CommonLib.Models.Category> categories = new List<CommonLib.Models.Category>();
                    List<CommonLib.Models.Publisher> publishers = new List<CommonLib.Models.Publisher>();


                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Get Method
                    HttpResponseMessage bookResponse = await client.GetAsync("gateway/book");
                    HttpResponseMessage categoryResponse = await client.GetAsync("gateway/category");
                    HttpResponseMessage publisherResponse = await client.GetAsync("gateway/publisher");


                    if (bookResponse.IsSuccessStatusCode && categoryResponse.IsSuccessStatusCode)
                    {
                        var bookResult = await bookResponse.Content.ReadAsStringAsync();
                        var categoryResult = await categoryResponse.Content.ReadAsStringAsync();
                        var publisherResult = await publisherResponse.Content.ReadAsStringAsync();

                        books = JsonConvert.DeserializeObject<List<CommonLib.Models.Book>>(bookResult);
                        categories = JsonConvert.DeserializeObject<List<CommonLib.Models.Category>>(categoryResult);
                        publishers = JsonConvert.DeserializeObject<List<CommonLib.Models.Publisher>>(publisherResult);

                        var bookRes = from book in books
                                      join category in categories on book.CategoryId equals category.CategoryId
                                      join publisher in publishers on book.PublisherId equals publisher.PublisherId

                                      //select new {book.BookName,book.BookContent,book.Price,book.PublishedDate,category.CategoryName,publisher.PublisherName};

                                      select (new DigitalBookStore.Web.Models.Book() { Id = book.BookId, Bookname = book.BookName, Bookcontent = book.BookContent, Price = book.Price, Publishername = publisher.PublisherName, Publisheddate = book.PublishedDate, Categoryname = category.CategoryName });

                        var authbooks = books.Where(x => x.UserId.ToString() == userIdVal)
                                        .Select(x =>
                                         new DigitalBookStore.Web.Models.Book()
                                         {
                                             Id = x.BookId,
                                             Bookname = x.BookName,
                                             Bookcontent = x.BookContent,
                                             Price = x.Price,
                                             Publishername = x.PublisherName,
                                             Publisheddate = x.PublishedDate,
                                             Categoryname = x.CategoryName,
                                             IsBlocked = x.IsBlocked,
                                         }).ToList();



                        //Searching

                        if (!String.IsNullOrEmpty(searchString))
                        {
                            bookRes = bookRes.Where(s => s.Bookname.ToLower().Contains(searchString.ToLower())
                                                   || s.Categoryname.ToLower().Contains(searchString.ToLower())
                                                   || s.Publishername.ToLower().Contains(searchString.ToLower())
                                                   || s.Publishername.ToLower().Contains(searchString.ToLower())
                                                   || s.Price.ToString().Contains(searchString)
                                                   || s.Publisheddate.ToLower().Contains(searchString));
                        }

                        //storting
                        switch (sortOrder)
                        {
                            case "bookName_desc":
                                bookRes = bookRes.OrderByDescending(x => x.Bookname);
                                break;

                            case "bookContent_desc":
                                bookRes = bookRes.OrderByDescending(x => x.Bookcontent);
                                break;

                            case "Price_desc":
                                bookRes = bookRes.OrderByDescending(x => x.Price);
                                break;

                            case "Price":
                                bookRes = bookRes.OrderBy(x => x.Price);
                                break;

                            case "publisherName_desc":
                                bookRes = bookRes.OrderByDescending(x => x.Publishername);
                                break;

                            case "categoryName_desc":
                                bookRes = bookRes.OrderByDescending(x => x.Categoryname);
                                break;

                            case "publisheddate_desc":
                                bookRes = bookRes.OrderByDescending(x => x.Publisheddate);
                                break;

                        }
                        if (userRole.ToLower() == "auther")
                        {
                            return View(authbooks);
                        }
                        else if (userRole.ToLower() == "reader")
                        {
                            return View(bookRes);
                        }
                        else
                        {
                            return View();
                        }


                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> GetSubscribedBooks()
        {
            return View();
        }

        public async Task<IActionResult> GetBookContent(Guid id)
        {
            try
            {
                string tokenVal = _context.HttpContext.Session.GetString("token");
                string userIdVal = _context.HttpContext.Session.GetString("userId");
                string email = "";
                Guid subscriptionId = Guid.NewGuid();

                using (var client = new HttpClient())
                {
                    CommonLib.Models.Book book = new CommonLib.Models.Book();

                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Get Method
                    HttpResponseMessage bookResponse = await client.GetAsync($"gateway/book/reader/{email}/books/{subscriptionId}/read");

                    var bookContent = await bookResponse.Content.ReadAsStringAsync();

                    book = JsonConvert.DeserializeObject<CommonLib.Models.Book>(bookContent);

                    var bookres = new DigitalBookStore.Web.Models.Book()
                    {

                        Bookcontent = book.BookContent,

                    };
                    return View(bookres);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        
        public async Task<IActionResult> AddBook(DigitalBookStore.Web.Models.Book book)
        {
            try
            {

                string tokenVal = _context.HttpContext.Session.GetString("token");

                string userIdVal = _context.HttpContext.Session.GetString("userId");

                Guid usernewId = new Guid(userIdVal);

                var baseUrlRes = BaseUrl + "gateway/book/addBook/" + userIdVal;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlRes);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    CommonLib.Models.Book addBook = new CommonLib.Models.Book
                    {
                        BookName = book.Bookname,
                        CategoryId = new Guid("66C42B33-17AB-4F30-B72A-B3B8B116C5D6"),
                        Active = true,
                        Price = (decimal)book.Price,
                        PublisherId = new Guid("928454CE-C488-4A58-BDBA-D3391192FCF1"),
                        PublishedDate = DateTime.Now.ToString(),
                        BookContent = book.Bookcontent,
                        UserId = new Guid(userIdVal),
                        IsBlocked = false,
                        CategoryName = book.Categoryname,
                        PublisherName = book.Publishername,
                        Category = new CommonLib.Models.Category { CategoryId = new Guid("66C42B33-17AB-4F30-B72A-B3B8B116C5D6"), CategoryName = book.Categoryname },
                        Publisher = new CommonLib.Models.Publisher { PublisherId = new Guid("928454CE-C488-4A58-BDBA-D3391192FCF1"), PublisherName = book.Publishername },
                        User = new CommonLib.Models.User
                        {
                            UserId = new Guid(userIdVal),
                            UserName = "string",
                            Password = "string",
                            RoleId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                            Email = "string",
                            UserType = "string",
                            FirstName = "string",
                            LastName = "string",
                            Role = new Role { RoleId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), RoleName = "Auther" }
                        }


                    };

                    var createBook = client.PostAsJsonAsync(baseUrlRes, addBook);
                    createBook.Wait();

                    if (createBook.IsCompletedSuccessfully)
                    {
                        return RedirectToAction("Index", "Book", new { @token = tokenVal });
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditBook(Guid id)
        {
            try
            {
                string tokenVal = _context.HttpContext.Session.GetString("token");
                string userIdVal = _context.HttpContext.Session.GetString("userId");

                using (var client = new HttpClient())
                {
                    CommonLib.Models.Book book = new CommonLib.Models.Book();

                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Get Method
                    HttpResponseMessage bookResponse = await client.GetAsync("gateway/book/" + id);

                    var bookContent = await bookResponse.Content.ReadAsStringAsync();

                    book = JsonConvert.DeserializeObject<CommonLib.Models.Book>(bookContent);

                    var bookres = new DigitalBookStore.Web.Models.Book()
                    {
                        Id = book.BookId,
                        Bookname = book.BookName,
                        Bookcontent = book.BookContent,
                        Categoryname = book.CategoryName,
                        Publishername = book.PublisherName,
                        Price = book.Price,
                        Publisheddate = book.PublishedDate
                    };

                    return View(bookres);

                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditBook(DigitalBookStore.Web.Models.Book book)
        {
            try
            {
                string tokenVal = _context.HttpContext.Session.GetString("token");

                string userIdVal = _context.HttpContext.Session.GetString("userId");

                Guid usernewId = new Guid(userIdVal);
                Guid book_id = new Guid(book.Id.ToString());

                var baseUrlRes = BaseUrl + "gateway/updateBook/auther/" + userIdVal + "/books/" + book_id;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlRes);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    CommonLib.Models.Book addBook = new CommonLib.Models.Book
                    {
                        BookName = book.Bookname,
                        CategoryId = new Guid("66C42B33-17AB-4F30-B72A-B3B8B116C5D6"),
                        Active = true,
                        Price = (decimal)book.Price,
                        PublisherId = new Guid("928454CE-C488-4A58-BDBA-D3391192FCF1"),
                        PublishedDate = DateTime.Now.ToString(),
                        BookContent = book.Bookcontent,
                        UserId = new Guid(userIdVal),
                        IsBlocked = false,
                        CategoryName = book.Categoryname,
                        PublisherName = book.Publishername,
                        Category = new CommonLib.Models.Category { CategoryId = new Guid("66C42B33-17AB-4F30-B72A-B3B8B116C5D6"), CategoryName = book.Categoryname },
                        Publisher = new CommonLib.Models.Publisher { PublisherId = new Guid("928454CE-C488-4A58-BDBA-D3391192FCF1"), PublisherName = book.Publishername },
                        User = new CommonLib.Models.User
                        {
                            UserId = new Guid(userIdVal),
                            UserName = "string",
                            Password = "string",
                            RoleId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                            Email = "string",
                            UserType = "string",
                            FirstName = "string",
                            LastName = "string",
                            Role = new Role { RoleId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), RoleName = "Auther" }
                        }
                    };

                    var createBook = client.PutAsJsonAsync(baseUrlRes, addBook);
                    createBook.Wait();

                    if (createBook.IsCompletedSuccessfully)
                    {
                        return RedirectToAction("Index", "Book", new { @token = tokenVal });
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult BlockUnblockBook(Guid bookid, bool isBlock)
        {
            try
            {
                string tokenVal = _context.HttpContext.Session.GetString("token");

                string userIdVal = _context.HttpContext.Session.GetString("userId");

                Guid usernewId = new Guid(userIdVal);
                Guid book_id = bookid;
                var uri = BaseUrl + $"gateway/blockBook/auther/{usernewId}/book/{book_id}";

                //var baseUrlRes = BaseUrl + "gateway/updateBook/auther/" + userIdVal + "/books/" + book_id;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var patchDoc = new JsonPatchDocument();

                    if (isBlock == true)
                    {
                        patchDoc.Replace("IsBlocked", "false");
                    }
                    else
                    {
                        patchDoc.Replace("IsBlocked", "true");
                    }
                    var serializedDoc = JsonConvert.SerializeObject(patchDoc);
                    var requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");


                    var createBook = client.PatchAsync(uri, requestContent);
                    createBook.Wait();

                    if (createBook.IsCompletedSuccessfully)
                    {
                        return RedirectToAction("Index", "Book", new { @token = tokenVal });
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<IActionResult> SubscribeBook(string bookid, string userid)
        {
            try
            {
                string tokenVal = _context.HttpContext.Session.GetString("token");
                string userIdVal = _context.HttpContext.Session.GetString("userId");

                Guid user_Id = new Guid(userIdVal);
                Guid book_id = new Guid(bookid);
                var uri = BaseUrl + $"gateway/book/{book_id}/subscribe/{user_Id}";

                //var baseUrlRes = BaseUrl + "gateway/updateBook/auther/" + userIdVal + "/books/" + book_id;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var subscribeBook = client.PostAsJsonAsync(uri, new { });
                    subscribeBook.Wait();

                    if (subscribeBook.IsCompletedSuccessfully)
                    {
                        return RedirectToAction("GetSubscriptionDetails", "Book");
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<IActionResult> GetSubscribedBookContent(Guid bookid)
        {
            //var likn = "gateway/reader/{emailid}/book/{subscription_id}/read";

            try
            {
                string tokenVal = _context.HttpContext.Session.GetString("token");
                string userId = _context.HttpContext.Session.GetString("userId");
                string emailid = _context.HttpContext.Session.GetString("userEmail");                

                using (var client = new HttpClient())
                {
                    List<CommonLib.Models.Subscription> subscription = new List<CommonLib.Models.Subscription>();

                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Get Method
                    HttpResponseMessage subscriptionResponse = await client.GetAsync("gateway/subscription/list");
                    var subscriptionContent = await subscriptionResponse.Content.ReadAsStringAsync();
                    subscription = JsonConvert.DeserializeObject<List<CommonLib.Models.Subscription>>(subscriptionContent);
                    var subscription_id = subscription.Where(x => x.BookId == bookid && x.UserId.ToString() == userId).FirstOrDefault().SubscriptionId;

                    HttpResponseMessage bookResponse = await client.GetAsync($"gateway/reader/{emailid}/book/{subscription_id}/read");
                    var bookResponseContent = await bookResponse.Content.ReadAsStringAsync();
                    var bookContent = JsonConvert.DeserializeObject<string>(bookResponseContent);

                    var bookContentRes = new DigitalBookStore.Web.Models.Book()
                    {                        
                        
                        Bookcontent = bookContent,                        
                    };

                    return View(bookContentRes);

                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<IActionResult> GetAllSubscribedBooks()
        {
            try
            {
                string tokenVal = _context.HttpContext.Session.GetString("token");
                string userId = _context.HttpContext.Session.GetString("userId");
                string emailId = _context.HttpContext.Session.GetString("userEmail");

                
                using (var client = new HttpClient())
                {
                    List<CommonLib.Models.Book> subscribedBooks = new List<CommonLib.Models.Book>();

                    client.BaseAddress = new Uri(BaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Get Method
                    HttpResponseMessage subscriptionResponse = await client.GetAsync($"/gateway/reader/{emailId}/books");
                    var subscribedBooksContent = await subscriptionResponse.Content.ReadAsStringAsync();
                    subscribedBooks = JsonConvert.DeserializeObject<List<CommonLib.Models.Book>>(subscribedBooksContent);

                    var bookRes = subscribedBooks.Select(
                        x => new DigitalBookStore.Web.Models.Book() 
                        { 
                            Id = x.BookId, 
                            Bookname = x.BookName, 
                            Bookcontent = x.BookContent, 
                            Price = x.Price, 
                            Categoryname = x.CategoryName,
                            Publishername = x.PublisherName, 
                            Publisheddate = x.PublishedDate, 
                            Image = x.Image 
                        }).ToList();

                    return View(bookRes);

                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }


    }
}
            


       
                


            