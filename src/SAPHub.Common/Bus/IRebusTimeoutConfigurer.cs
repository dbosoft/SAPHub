using Rebus.Config;
using Rebus.Timeouts;

namespace SAPHub.Bus;

public interface IRebusTimeoutConfigurer
{
    void Configure(StandardConfigurer<ITimeoutManager> configurer);
}