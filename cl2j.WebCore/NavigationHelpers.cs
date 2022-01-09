using Microsoft.AspNetCore.Mvc;

namespace cl2j.WebCore
{
    public static class NavigationHelpers
    {
        public static string Domain { get; set; } = "";

        public static string GetCanonicalUrl(this IUrlHelper url, string canonicalUrl)
        {
            if (string.IsNullOrEmpty(canonicalUrl))
            {
                var request = url.ActionContext.HttpContext.Request;
                return Domain + request.Path + request.QueryString.ToString();
            }

            return Domain + canonicalUrl;
        }
    }
}