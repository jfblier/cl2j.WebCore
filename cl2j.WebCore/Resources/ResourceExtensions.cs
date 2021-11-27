using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace cl2j.WebCore.Resources
{
    public static class ResourceExtensions
    {
        public static void AddResources(this IServiceCollection services, IConfiguration configuration, string defaultLanguage = "en", string configurationSectionName = "cl2j:ResourceOptions")
        {
            services.Configure<ResourceOptions>(options => configuration.GetSection(configurationSectionName).Bind(options));

            services.AddResources(defaultLanguage);
        }

        public static void AddResources(this IServiceCollection services, string defaultLanguage = "en")
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<Microsoft.Extensions.Localization.IStringLocalizer, ResourceManagerStringLocalizer>();

            if (!string.IsNullOrEmpty(defaultLanguage))
                ResourceManagerStringLocalizer.DefaultLanguage = defaultLanguage;

            services.TryAddSingleton<IResourceService, ResourceService>();
        }
    }
}