using System.Threading.Tasks;

namespace cl2j.WebCore.Resources
{
    public interface IResourceService
    {
        Task<ResourceCollection> GetResourcesAsync();
    }
}