using cl2j.FileStorage;
using cl2j.WebCore.Resources;
using cl2j.WebCore.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cl2j.WebCore
{
    public class Startup
    {
        private readonly IWebHostEnvironment environment;
        private readonly IConfigurationRoot configuration;

        public Startup(IWebHostEnvironment environment)
        {
            this.environment = environment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(environment);
            services.AddSingleton(configuration);
            services.AddSingleton<IConfiguration>(configuration);

            //Bootstrap the FileStorage to be available from DependencyInjection. This will allow accessing IFileStorageProviderFactory instance
            services.AddFileStorage();

            //Configure the resources. Override the default language to be fr (French)
            services.AddResources(configuration, "fr");

            //Configure the route system.
            services.AddRoutes(configuration);

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(async endpoints =>
            {
                await app.ApplicationServices.UseRoutesAsync(endpoints);

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}