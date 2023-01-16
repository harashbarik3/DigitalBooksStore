using CommonLib.Models;

namespace BookService.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
        Task<Subscription> AddSubscriptionAsync(Subscription subscription);

        Task<Subscription> AddSubscriptionWithBookidAsync(Guid bookid, Guid userid);

        Task<Subscription> CancleSubscriptionAsync(string email, Guid subscription_id);
    }
}
