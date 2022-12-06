using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
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

        private ILogger _logger;


        public RebusTransportSelector(IConfiguration configuration, InMemNetwork network, ILogger<RebusTransportSelector> logger)
        {
            _network = network;
            _logger = logger;

            _busType = configuration["bus:type"];
            _connectionString = configuration["bus:connectionstring"]; 

            if (_busType == null)
                throw new InvalidOperationException("Missing configuration entry for bus::type. Configure a valid bus type (inmemory,rabbitmq,azurestorage,azureservicebus");

            switch (_busType)
            {
                case "azurestorage":
                case "azureservicebus":
                case "rabbitmq":
                    if (_connectionString == null)
                        throw new InvalidOperationException("Missing configuration entry for bus::connectionstring.");
                    return;

                case "inmemory": return;

            }

            throw new InvalidOperationException($"Invalid bus type: {_busType} .Configure a valid bus type (inmemory,rabbitmq,azurestorage,azureservicebus).");

        }

        public void ConfigureAsOneWayClient(StandardConfigurer<ITransport> configurer)
        {
            
            switch (_busType)
            {
                case "azurestorage":
                    configurer.UseAzureStorageQueuesAsOneWayClient(_connectionString);
                        
                    break;
                case "azureservicebus":
                    configurer.UseAzureServiceBusAsOneWayClient(_connectionString);
                        
                    break;
                case "rabbitmq":
                    WaitForRabbitMq(_connectionString);

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
                case "azurestorage":
                    configurer.UseAzureStorageQueues(_connectionString, queueName);
                        
                    break;
                case "azureservicebus":
                    configurer.UseAzureServiceBus(_connectionString,queueName);
                    break;
                case "rabbitmq":

                    WaitForRabbitMq(_connectionString);
                    configurer.UseRabbitMq(_connectionString,queueName);
                    break;

                case "inmemory":
                    configurer.UseInMemoryTransport(_network,queueName);
                    break;
            }
        }

        private void WaitForRabbitMq(string connectionString)
        {
            const int maxRetry = 5;
            var retries = maxRetry;
            var retryNo = 0;
            var factory = new ConnectionFactory(){Uri= new Uri(connectionString)};

            while (true)
            {
                try
                {
                    using (var _ = factory.CreateConnection())
                    {
                        break;
                    }
                }
                catch
                {
                    if (retries == 1)
                    {
                        _logger.LogError("Failed to connect to RabbitMq. Giving up...");
                        throw;
                    }

                    retries--;
                    retryNo++;
                    _logger.LogInformation($"Failed to connect to RabbitMq.Trying again in {retryNo*5} seconds... ({retryNo}/{maxRetry})");
                    Thread.Sleep(5000* retryNo);

                }
            }
        }

    }
}
