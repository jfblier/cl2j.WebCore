using cl2j.WebCore.Routes;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelpers
    {
        public static string Language(this IHtmlHelper htmlHelper)
        {
            var localizer = htmlHelper.GetStringLocalizer();
            return localizer.GetLanguage(htmlHelper.ViewContext.HttpContext.Request);
        }

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

        public static string Pluralize(int? count, string text)
        {
            return text + (count.HasValue && count > 1 ? "s" : string.Empty);
        }

        public static IHtmlContent Localize(this IHtmlHelper htmlHelper, string resourceName, params object[] values)
        {
            var localizer = htmlHelper.GetStringLocalizer();
            var resourceValue = localizer?.Localize(resourceName, values);

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

        private static string GetLanguage(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.GetStringLocalizer()?.GetLanguage(htmlHelper.ViewContext.HttpContext.Request);
        }

        #endregion Private
    }
}