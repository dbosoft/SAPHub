using Rebus.Config;
using Rebus.DataBus;

namespace SAPHub.Bus;

public interface IRebusDataBusConfigurer
{
    void Configure(StandardConfigurer<IDataBusStorage> configurer);
}