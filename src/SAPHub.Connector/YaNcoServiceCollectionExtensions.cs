using System;
using Dbosoft.YaNco;
using Dbosoft.YaNco.TypeMapping;
using Microsoft.Extensions.DependencyInjection;
using SAPHub.Connector;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class YaNcoServiceCollectionExtensions
    {

        public static IServiceCollection AddYaNco(this IServiceCollection services)
        {
            services.AddSingleton<ILogger, RfcLoggingAdapter>();
            services.AddSingleton<Func<ILogger, IFieldMapper, IRfcRuntime>>(
                sp => (l, m) => new RfcRuntime(sp.GetService<ILogger>(), m));
            services.AddSingleton<SAPConnectionFactory>();
            services.AddSingleton(sp => sp.GetRequiredService<SAPConnectionFactory>().CreateConnectionFunc());
            services.AddTransient<IRfcContext, RfcContext>();

            return services;
        }
    }
}