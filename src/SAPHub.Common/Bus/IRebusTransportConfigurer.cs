﻿using System.Collections.Generic;
using System.Linq;
using Rebus.Config;
using Rebus.Transport;

namespace SAPHub
{
    public interface IRebusTransportConfigurer
    {
        void ConfigureAsOneWayClient(StandardConfigurer<ITransport> configurer);
        void Configure(StandardConfigurer<ITransport> configurer, string queueName);
    }
}
