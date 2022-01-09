using cl2j.DataStore.Core.Cache;
using cl2j.FileStorage.Core;
using cl2j.FileStorage.Extensions;
using cl2j.Tooling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace cl2j.WebCore.Resources
{
    public class ResourceService : IResourceService
    {
        private readonly CacheLoader cacheLoader;
        private ResourceCollection resourceCollection = new(new Dictionary<string, Localized<string>>());

        public ResourceService(IFileStorageFactory fileStorageFactory, IOptions<ResourceOptions> options, ILogger<ResourceService> logger)
        {
            if (string.IsNullOrEmpty(options.Value.DataStoreName))
                throw new Exception("ResourceOptions DataStoreName is missing.");
            if (options.Value.Languages == null || options.Value.Languages.Count == 0)
                throw new Exception("ResourceOptions Languages is missing.");

            cacheLoader = new CacheLoader("ResourceService", options.Value.RefreshInterval, async () =>
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    var fileStorageProvider = fileStorageFactory.Get(options.Value.DataStoreName);
                    if (fileStorageProvider == null)
                        throw new InvalidOperationException();

                    //Load each language resource file
                    var resources = new Dictionary<string, Localized<string>>();
                    foreach (var language in options.Value.Languages)
                    {
                        var resourcesOfLanguage = await fileStorageProvider.ReadJsonObjectAsync<Dictionary<string, string>>($"resources.{language}.json");

                        foreach (var kvp in resourcesOfLanguage)
                        {
                            if (resources.TryGetValue(kvp.Key, out var resource))
                                resource.TryAdd(language, kvp.Value);
                            else
                                resources.Add(kvp.Key, new Localized<string> { { language, kvp.Value } });
                        }
                    }

                    //Create the ResourceCollection from the resource loaded
                    try
                    {
                        resourceCollection = new ResourceCollection(resources);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Unexpected error at {nameof(GetResourcesAsync)}");
                    }

#pragma warning disable CA2254 // Template should be a static expression
                    logger.LogDebug($"ResourceService --> {resources.Count} resources in {sw.ElapsedMilliseconds}ms");
#pragma warning restore CA2254 // Template should be a static expression
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Unexpected error at {nameof(GetResourcesAsync)}");
                }
            }, logger);
        }

        public async Task<ResourceCollection> GetResourcesAsync()
        {
            await cacheLoader.WaitAsync();
            return resourceCollection;
        }
    }
}