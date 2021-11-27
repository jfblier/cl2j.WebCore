using cl2j.WebCore.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace cl2j.WebCore.Resources
{
    public class ResourceManagerStringLocalizer : IStringLocalizer
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IResourceService resourceService;
        private readonly IRouteService routeService;
        internal static string DefaultLanguage;

        public ResourceManagerStringLocalizer(IHttpContextAccessor httpContextAccessor, IResourceService resourceService, IRouteService routeService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.resourceService = resourceService;
            this.routeService = routeService;
        }

        public LocalizedString this[string name] => GetValue(name);

        public LocalizedString this[string name, params object[] arguments] => GetValue(name, arguments);

        public string GetLanguage(HttpRequest request)
        {
            var routeMatch = routeService.GetRouteWithUrlAsync(request.Path).Result;
            if (routeMatch != null)
                return routeMatch.Language;

            return DefaultLanguage;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var language = GetLanguage(httpContextAccessor.HttpContext.Request);

            var result = new List<LocalizedString>();
            var resourceCollection = resourceService.GetResourcesAsync().Result;
            foreach (var entry in resourceCollection.GetAll(language))
                result.Add(new LocalizedString(entry.Key, entry.Value));
            return result;
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region Private

        private LocalizedString GetValue(string name, params object[] arguments)
        {
            var language = GetLanguage(httpContextAccessor.HttpContext.Request);

            var resourceCollection = resourceService.GetResourcesAsync().Result;
            var value = resourceCollection.Get(name, language, arguments);

            return new LocalizedString(name, value);
        }

        #endregion Private
    }
}