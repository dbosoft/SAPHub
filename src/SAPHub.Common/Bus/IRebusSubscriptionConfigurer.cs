using Rebus.Config;
using Rebus.Subscriptions;

namespace SAPHub
{
    public interface IRebusSubscriptionConfigurer
    {
        void Configure(StandardConfigurer<ISubscriptionStorage> timeoutManager);
    }
}