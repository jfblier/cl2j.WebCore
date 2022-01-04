using cl2j.DataStore.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cl2j.WebCore.Routes
{
    public class RouteService : IRouteService
    {
        private readonly IDataStoreList<string, Route> dataStore;
        private readonly ILogger logger;

        public RouteService(IDataStoreList<string, Route> dataStore, ILogger<RouteService> logger)
        {
            this.dataStore = dataStore;
            this.logger = logger;
        }

        public async Task<IEnumerable<Route>> GetAllAsync()
        {
            try
            {
                var routes = await dataStore.GetAllAsync();
                return routes;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unexpected error");
                return null;
            }
        }

        public async Task<Route> GetRouteAsync(string routeName)
        {
            var routes = await GetAllAsync();
            return routes.FirstOrDefault(r => r.Id == routeName);
        }

        public async Task<RouteMatch> GetRouteWithUrlAsync(string url)
        {
            url = CleanUrl(url);

            var routes = await GetAllAsync();
            foreach (var route in routes)
            {
                foreach (var pattern in route.Pattern)
                {
                    if (pattern.Value == url)
                        return new RouteMatch { Route = route, Language = pattern.Key };
                }
            }
            return null;
        }

        private static string CleanUrl(string url)
        {
            var n = url.IndexOf("?");
            if (n >= 0)
                url = url[(n + 1)..];
            if (url.StartsWith("/"))
                url = url[1..];
            url = url.ToLower();
            return url;
        }
    }
}