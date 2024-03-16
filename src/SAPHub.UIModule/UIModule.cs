using Dbosoft.Hosuto.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SAPHub.UI;

public class UIModule: WebModule
{
    public override string Path => _endpointResolver.GetEndpoint("ui").ToString();

    private readonly EndpointResolver _endpointResolver;

    public UIModule(IConfiguration configuration)
    {
        Configuration = configuration;
        _endpointResolver = new EndpointResolver(Configuration);
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    // ReSharper disable once UnusedMember.Global
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddSingleton(_endpointResolver);

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();


        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("index.html");
        });
    }

}