using Rebus.Config;
using Rebus.Subscriptions;

namespace SAPHub.Bus
{
    public interface IRebusSubscriptionConfigurer
    {
        void Configure(StandardConfigurer<ISubscriptionStorage> configure);
    }
}