using System;
using System.Collections.Generic;
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
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.Serialization.Json;
using SAPHub.Bus;
using SAPHub.Messages;
using SAPHub.StateDb;

namespace SAPHub.ApiModule
{
    /// <summary>
    /// This is the module the SAPHub REST API
    /// </summary>
    public class ApiModule : WebModule
    {
        public override string Path => _endpointResolver.GetEndpoint("api").ToString();
        private readonly EndpointResolver _endpointResolver;

        public ApiModule(IConfiguration configuration)
        {
            _endpointResolver = new EndpointResolver(configuration);
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceProvider sp, IServiceCollection services)
        {
            //configure CORS for UI, this will allow ui endpoints to use
            //api from their origin
            services.AddCors(o => o.AddPolicy("ui", builder =>
            {
                //remove path from endpoint address
                var uiEndpoint = _endpointResolver.GetEndpoint("ui").GetComponents(UriComponents.SchemeAndServer,
                    UriFormat.SafeUnescaped);
                builder.WithOrigins(uiEndpoint);
            }));

            services.AddControllers();

            //setup swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SAPHub.ApiModule", Version = "v1" });
                c.EnableAnnotations();

                // XML Documentation
                var xmlFile = $"{typeof(ApiModule).Assembly.GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });

            //setup rebus as message sender and event recipient
            //transport will be configured by IRebusTransportConfigurer, so app has control here
            services.AddRebus(configurer =>
            {
                return configurer
                    .Transport(t =>
                        sp.GetRequiredService<IRebusTransportConfigurer>().Configure(t, QueueNames.Api))
                    .Routing(x =>
                    {
                        x.TypeBased()
                            .Map(typeof(GetCompanyCodesCommand), QueueNames.SAPConnector)
                            .Map(typeof(GetCompanyCodeCommand), QueueNames.SAPConnector)
                            .Map<OperationStatusEvent>(QueueNames.Api);
                        
                    })
                    .Options(x =>
                    {
                        x.RetryStrategy();
                        x.SetNumberOfWorkers(5);
                    })
                    .DataBus(ds => sp.GetRequiredService<IRebusDataBusConfigurer>()
                        .Configure(ds)
                    )
                    .Subscriptions(s => sp.GetRequiredService<IRebusSubscriptionConfigurer>().Configure(s))
                    .Serialization(x => x.UseNewtonsoftJson(new JsonSerializerSettings
                        { TypeNameHandling = TypeNameHandling.None }))
                    .Timeouts(cfg => sp.GetRequiredService<IRebusTimeoutConfigurer>().Configure(cfg))
                    .Logging(x => x.ColoredConsole());

            }, onCreated: bus => bus.Subscribe(typeof(OperationStatusEvent)));

            services.AddRebusHandler<OperationStatusEventHandler>();

            //setup state db (efcore)
            services.AddScoped((_) =>
            {
                //the context has to be build by app, so it has control of actual implementation selection
                var optionsBuilder = new DbContextOptionsBuilder<StateStoreContext>();
                sp.GetRequiredService<IDbContextConfigurer<StateStoreContext>>().Configure(optionsBuilder);
                return new StateStoreContext(optionsBuilder.Options, sp.GetRequiredService<IModelBuilder<StateStoreContext>>());
            });

            //repository abstraction for StateStore
            services.AddScoped(typeof(IStateStoreRepository<>), typeof(StateStoreRepository<>));

            //add a handler that will create database if not existing
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

            //swagger ceremony
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DisplayOperationId();
                c.SwaggerEndpoint($"{Path.TrimEnd('/')}/swagger/v1/swagger.json", "SAPHub.ApiModule v1");

        });

            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new
                    List<string> { "index.html" }
            });
            app.UseStaticFiles();
            app.UseRouting();

            //enable cors policy from above
            app.UseCors("ui");

            //currently not used, but keep it possible
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }

    }

}
