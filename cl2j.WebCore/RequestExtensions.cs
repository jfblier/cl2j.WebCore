using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace cl2j.WebCore
{
    public static class RequestExtensions
    {
        public static string GetLanguage(this HttpRequest request)
        {
            return request.GetStringLocalizer()?.GetLanguage(request);
        }

        public static string Localize(this HttpRequest request, string resourceName, params object[] values)
        {
            return request.GetStringLocalizer()?.Localize(resourceName, values);
        }

        private static cl2j.WebCore.Resources.ResourceManagerStringLocalizer GetStringLocalizer(this HttpRequest request)
        {
            return request.HttpContext.RequestServices.GetService(typeof(IStringLocalizer)) as cl2j.WebCore.Resources.ResourceManagerStringLocalizer;
        }
    }
}