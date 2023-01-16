using DigitalBookStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace DigitalBookStore.Web.Controllers
{
    public class SubscriptionDashboardController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                string uri = "http://localhost:7024/api/subscriptions";
                string apiUri = "https://azurefunctiongetrecords20230111162832.azurewebsites.net/api/subscriptions?code=W9iuU6Tm3sgdGAI7J8G-QW1xjgXyxJ8O9dFWxfVCH5KiAzFubvAEWA==";
                using (var client = new HttpClient())
                {
                    List<SubscriptionDetail> subscriptions = new List<SubscriptionDetail>();

                    client.BaseAddress = new Uri(apiUri);
                    client.DefaultRequestHeaders.Clear();
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenVal);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Get Method
                    HttpResponseMessage bookResponse = await client.GetAsync(apiUri);

                    var subscriptionList = await bookResponse.Content.ReadAsStringAsync();

                    //var result = subscriptionList.ToString();

                    //var jobj = JObject.Parse(subscriptionList);

                    subscriptions = JsonConvert.DeserializeObject<List<SubscriptionDetail>>(JsonConvert.DeserializeObject<string>(subscriptionList));

                    //SubscriptionDetail subs = new SubscriptionDetail()
                    //{
                    //    Id =subscriptions.Id.ToString(),
                    //    SubscriptionDate = subscriptions.SubscriptionDate,
                    //    BookName = subscriptions.BookName,
                    //    auther = subscriptions.auther,
                    //    Username = subscriptions.Username,
                    //    userEmail = subscriptions.userEmail

                    //};

                    return View(subscriptions);
                }

            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
