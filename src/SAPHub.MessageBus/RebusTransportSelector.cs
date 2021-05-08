using System;
using Microsoft.Extensions.Configuration;
using Rebus.Config;
using Rebus.Transport;
using Rebus.Transport.InMem;
using SAPHub.Bus;

namespace SAPHub.MessageBus
{
    public class RebusTransportSelector : IRebusTransportConfigurer
    {
        private readonly InMemNetwork _network;
        private readonly string _busType;
        private readonly string _connectionString;

        public RebusTransportSelector(IConfiguration configuration, InMemNetwork network)
        {
            _network = network;

            _busType = configuration["bus:type"];
            _connectionString = configuration["bus:connectionstring"]; 

            if (_busType == null)
                throw new InvalidOperationException("Missing configuration entry for bus::type. Configure a valid bus type (inmemory,rabbitmq,azureservicebus");

            switch (_busType)
            {
                case "azureservicebus":
                case "rabbitmq":
                    if (_connectionString == null)
                        throw new InvalidOperationException("Missing configuration entry for bus::connectionstring.");
                    return;

                case "inmemory": return;

            }

            throw new InvalidOperationException($"Invalid bus type: {_busType} .Configure a valid bus type (inmemory,rabbitmq,azureservicebus).");

        }

        public void ConfigureAsOneWayClient(StandardConfigurer<ITransport> configurer)
        {
            
            switch (_busType)
            {
                case "azureservicebus":
                    configurer.UseAzureServiceBusAsOneWayClient(_connectionString);
                        
                    break;
                case "rabbitmq":
                    configurer.UseRabbitMqAsOneWayClient(_connectionString)
                        .InputQueueOptions(o => o.SetAutoDelete(autoDelete: true));
                    break;

                case "inmemory":
                    configurer.UseInMemoryTransportAsOneWayClient(_network);
                    break;
            }
        }

        public void Configure(StandardConfigurer<ITransport> configurer, string queueName)
        {
            switch (_busType)
            {
                case "azureservicebus":
                    configurer.UseAzureServiceBus(_connectionString,queueName);
                    break;
                case "rabbitmq":
                    configurer.UseRabbitMq(_connectionString,queueName);
                    break;

                case "inmemory":
                    configurer.UseInMemoryTransport(_network,queueName);
                    break;
            }
        }

    }
}
