using cl2j.WebCore.Resources;
using cl2j.WebCore.Routes;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelpers
    {
        public static IHtmlContent Route(this IHtmlHelper htmlHelper, string routeName, string queryString = null)
        {
            var url = routeName;

            var routeService = htmlHelper.GetRouteService();
            if (routeService != null)
            {
                var route = routeService.GetRouteAsync(routeName).Result;
                if (route == null)
                    return null;

                var language = htmlHelper.GetLanguage();

                url = route.Pattern[language];
                if (queryString != null)
                    url += (queryString.StartsWith('?') ? string.Empty : "?") + queryString;
                url = ToAbsolute(url);
            }

            return new HtmlString(url);
        }

        public static IHtmlContent AlternateRoute(this IHtmlHelper htmlHelper)
        {
            var routeService = htmlHelper.GetRouteService();
            if (routeService == null)
                return new HtmlString(string.Empty);

            var url = htmlHelper.ViewContext.HttpContext.Request.Path;
            var query = htmlHelper.ViewContext.HttpContext.Request.QueryString;
            var routeMatch = routeService.GetRouteWithUrlAsync(url).Result;
            if (routeMatch != null)
            {
                var language = htmlHelper.GetLanguage();
                foreach (var kvp in routeMatch.Route.Pattern)
                {
                    if (kvp.Key != language)
                        return new HtmlString(ToAbsolute(kvp.Value + query));
                }
            }

            return new HtmlString(url);
        }

        public static IHtmlContent Localize(this IHtmlHelper htmlHelper, string resourceName, params object[] values)
        {
            var resourceValue = Localize(htmlHelper, resourceName);

            if (values != null && values.Length > 0)
                resourceValue = string.Format(resourceValue, values);

            return htmlHelper.Raw(resourceValue);
        }

        public static IHtmlContent LocalizeRaw(this IHtmlHelper htmlHelper, string resourceName, params object[] values)
        {
            return Localize(htmlHelper, resourceName, values);
        }

        #region Private

        private static string ToAbsolute(string url)
        {
            if (url.StartsWith("/") || url.StartsWith("http"))
                return url;
            return "/" + url;
        }

        private static string Localize(IHtmlHelper htmlHelper, string resourceName)
        {
            var localizer = htmlHelper.GetStringLocalizer();
            if (localizer != null)
            {
                var resourceValue = localizer[resourceName];
                if (resourceValue != null)
                    return resourceValue.Value;
            }

            return resourceName;
        }

        private static cl2j.WebCore.Resources.ResourceManagerStringLocalizer GetStringLocalizer(this IHtmlHelper htmlHelper)
        {
            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            return services.GetService<IStringLocalizer>() as cl2j.WebCore.Resources.ResourceManagerStringLocalizer;
        }

        private static IRouteService GetRouteService(this IHtmlHelper htmlHelper)
        {
            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            return services.GetService<IRouteService>();
        }

        private static IResourceService GetResourceService(this IHtmlHelper htmlHelper)
        {
            var services = htmlHelper.ViewContext.HttpContext.RequestServices;
            return services.GetService<IResourceService>();
        }

        private static string GetLanguage(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.GetStringLocalizer()?.GetLanguage(htmlHelper.ViewContext.HttpContext.Request);
        }

        #endregion Private
    }
}