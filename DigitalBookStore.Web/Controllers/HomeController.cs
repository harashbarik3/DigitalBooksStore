using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace DigitalBookStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _context;
        public HomeController(IHttpContextAccessor context)
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

                                      select (new DigitalBookStore.Web.Models.Book() { Bookname = book.BookName, Bookcontent = book.BookContent, Price = book.Price, Publishername = publisher.PublisherName, Publisheddate = book.PublishedDate, Categoryname = category.CategoryName });

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

                        return View(bookRes);
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
    }
}
