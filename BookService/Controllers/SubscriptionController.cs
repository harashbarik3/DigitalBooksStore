using BookService.Repositories;
using CommonLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookService.Controllers
{
    [Route("api/v1/digitalbooks/[controller]")]
    [ApiController]
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        public SubscriptionController(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var roles = await _subscriptionRepository.GetAllSubscriptionsAsync();

            return Ok(roles);
        }

        [HttpPost("{subscribe}")]
        public async Task<IActionResult> AddSubscriptions([FromBody] Subscription subscription)
        {
            var aadSubscription = new Subscription()
            {
                SubscriptionDate = subscription.SubscriptionDate,
                UserId= subscription.UserId,
                BookId= subscription.BookId                
            };
            var subscriptionRes = await _subscriptionRepository.AddSubscriptionAsync(aadSubscription);

            return Ok(subscriptionRes);
        }

        [HttpPost("{book_id}/subscribe/{user_id}")]
        public async Task<IActionResult> AddSubscriptionwithBookId([FromRoute]Guid book_id, [FromRoute]Guid user_Id)
        {
            var addSubscription = await _subscriptionRepository.AddSubscriptionWithBookidAsync(book_id, user_Id);

            return Ok(addSubscription);
        }

        [HttpPost("readers/{emailid}/book/{subscription_id}/cancle-subscription")]
        public async Task<IActionResult> CancleSubscription( string emailid, Guid subscription_id)
        {
            try
            {
                var cancleSubs = await _subscriptionRepository.CancleSubscriptionAsync(emailid, subscription_id);

                if (cancleSubs != null)
                {
                    return Ok(cancleSubs);
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
    }
}
