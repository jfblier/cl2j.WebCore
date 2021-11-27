﻿using cl2j.DataStore.Core;
using cl2j.DataStore.Core.Cache;
using cl2j.DataStore.Json;
using cl2j.FileStorage.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

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
            services.AddSingleton<IDataStoreList<string, Route>>(builder =>
            {
                var logger = builder.GetRequiredService<ILogger<DataStoreCacheList<string, Route>>>();
                var fileStorageFactory = builder.GetRequiredService<IFileStorageFactory>();
                var routeOptions = builder.GetRequiredService<IOptions<RouteOptions>>().Value;
                var fileStorageProvider = fileStorageFactory.Get(routeOptions.DataStoreName);
                if (fileStorageProvider == null)
                    throw new System.Exception($"RouteExtensions : DataStore '{routeOptions.DataStoreName}' not found");

                var dataStore = new DataStoreListJson<string, Route>(fileStorageProvider, "routes.json", r => r.Id);
                var dataStoreCache = new DataStoreCacheList<string, Route>("Routes", dataStore, routeOptions.RefreshInterval, r => r.Id, logger);

                return dataStoreCache;
            });

            services.AddSingleton<IRouteService, RouteService>();
        }

        public static async Task UseRoutesAsync(this IServiceProvider serviceProvider, Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            var routes = await serviceProvider.GetRequiredService<IRouteService>().GetAllAsync();
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