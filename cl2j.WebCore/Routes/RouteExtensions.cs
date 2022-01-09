using cl2j.DataStore.Core;
using cl2j.DataStore.Core.Cache;
using cl2j.DataStore.Json;
using cl2j.FileStorage.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cl2j.WebCore.Routes
{
    public static class RouteExtensions
    {
        public static void AddRoutes(this IServiceCollection services, IConfiguration configuration, string configurationSectionName = "cl2j:RouteOptions")
        {
            services.Configure<RouteOptions>(options => configuration.GetSection(configurationSectionName).Bind(options));

            services.AddRoutes();
        }

        public static void AddRoutes(this IServiceCollection services)
        {
            services.AddSingleton<IDataStore<string, Route>>(builder =>
            {
                var logger = builder.GetRequiredService<ILogger<DataStoreCache<string, Route>>>();
                var fileStorageFactory = builder.GetRequiredService<IFileStorageFactory>();
                var routeOptions = builder.GetRequiredService<IOptions<RouteOptions>>().Value;
                var fileStorageProvider = fileStorageFactory.Get(routeOptions.DataStoreName);
                if (fileStorageProvider == null)
                    throw new System.Exception($"RouteExtensions : DataStore '{routeOptions.DataStoreName}' not found");

                var dataStore = new DataStoreJson<string, Route>(fileStorageProvider, "routes.json", r => r.Id, logger);
                var dataStoreCache = new DataStoreCache<string, Route>("Routes", dataStore, routeOptions.RefreshInterval, r => r.Id, logger);

                return dataStoreCache;
            });

            services.AddSingleton<IRouteService, RouteService>();
        }

        public static async Task UseRoutesAsync(this IServiceProvider serviceProvider, Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            var routeService = serviceProvider.GetRequiredService<IRouteService>();
            var routes = await routeService.GetAllAsync();
            foreach (var route in routes)
            {
                foreach (var kvpPattern in route.Pattern)
                {
                    var language = kvpPattern.Key;
                    var pattern = kvpPattern.Value;
                    endpoints.MapControllerRoute($"{route.Id}_{language}", pattern, new { controller = route.Controller, action = route.Action });
                }
            }
        }
    }
}