using System;
using Dbosoft.YaNco;
using Dbosoft.YaNco.TypeMapping;
using Microsoft.Extensions.DependencyInjection;
using SAPHub.ConnectorModule;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class YaNcoServiceCollectionExtensions
    {

        // ReSharper disable once UnusedMethodReturnValue.Global
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