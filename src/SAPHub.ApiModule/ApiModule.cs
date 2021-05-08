using System;
using System.Threading.Tasks;
using Dbosoft.Hosuto.HostedServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Dbosoft.Hosuto.Modules;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.Serialization.Json;
using Rebus.ServiceProvider;
using SAPHub.Bus;
using SAPHub.Messages;
using SAPHub.StateDb;

namespace SAPHub.ApiModule
{
    public class ApiModule : WebModule
    {
        public override string Path => "api";
        public override string Name  => nameof(ApiModule);


        public ApiModule(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public void ConfigureServices(IServiceProvider sp, IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SAPHub.ApiModule", Version = "v1" });
                c.EnableAnnotations();

            });

            services.AddRebus(configurer =>
            {
                 return configurer
                    .Transport(t =>
                        sp.GetRequiredService<IRebusTransportConfigurer>().Configure(t, QueueNames.Api))
                    .Routing(x =>
                    {
                        x.TypeBased()
                            .Map(typeof(GetCompaniesCommand), QueueNames.SAPConnector)
                            .Map(typeof(GetCompanyCommand), QueueNames.SAPConnector);
                    })
                    .Options(x =>
                    {
                        x.SimpleRetryStrategy();
                        x.SetNumberOfWorkers(5);
                    })
                    .Subscriptions(s => sp.GetRequiredService<IRebusSubscriptionConfigurer>().Configure(s))
                    .Serialization(x => x.UseNewtonsoftJson(new JsonSerializerSettings
                        { TypeNameHandling = TypeNameHandling.None }))
                    .Logging(x => x.ColoredConsole());

            });

            services.AddRebusHandler<OperationStatusEventHandler>();

            services.AddScoped((_) =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<StateStoreContext>();
                sp.GetRequiredService<IDbContextConfigurer<StateStoreContext>>().Configure(optionsBuilder);
                return new StateStoreContext(optionsBuilder.Options, sp.GetRequiredService<IModelBuilder<StateStoreContext>>());
            });

            services.AddScoped(typeof(IStateStoreRepository<>), typeof(StateStoreRepository<>));
            services.AddHostedHandler((p, _) =>
            {
                using var context = p.GetRequiredService<StateStoreContext>();
                context.Database.EnsureCreated();
                return Task.CompletedTask;
            });


            services.AddScoped<IOperationService, OperationService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DisplayOperationId();
                c.SwaggerEndpoint($"/{Path}/swagger/v1/swagger.json", "SAPHub.ApiModule v1");

        });

            app.ApplicationServices.UseRebus(bus => bus.Subscribe(typeof(OperationStatusEvent)));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
