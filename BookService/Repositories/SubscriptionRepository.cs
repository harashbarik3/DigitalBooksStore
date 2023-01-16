using CommonLib.Models;
using Microsoft.EntityFrameworkCore;
using UserServiceLib;

namespace BookService.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly DigitalbookstoreContext _context;
        public SubscriptionRepository(DigitalbookstoreContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        public async Task<Subscription> AddSubscriptionAsync(Subscription subscription)
        {
            subscription.SubscriptionId = Guid.NewGuid();
            await _context.AddAsync(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }
        public async Task<Subscription> AddSubscriptionWithBookidAsync(Guid bookid, Guid userid)
        {
            try
            {
                var subscription = new Subscription();
                subscription.SubscriptionId = Guid.NewGuid();
                subscription.BookId = bookid;
                subscription.UserId = userid;
                subscription.SubscriptionDate = DateTime.UtcNow.ToString();
                await _context.AddAsync(subscription);
                await _context.SaveChangesAsync();

                return subscription;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<Subscription> CancleSubscriptionAsync(string email, Guid subscription_id)
        {
            try
            {
                var existingSubscription = await _context.Subscriptions.FindAsync(subscription_id);

                if (existingSubscription != null)
                {
                    _context.Subscriptions.Remove(existingSubscription);
                    await _context.SaveChangesAsync();
                    return existingSubscription;
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
