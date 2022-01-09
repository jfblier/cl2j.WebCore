namespace cl2j.WebCore.Routes
{
    public interface IRouteService
    {
        Task<IEnumerable<Route>> GetAllAsync();

        Task<Route?> GetRouteAsync(string routeName);

        Task<RouteMatch?> GetRouteWithUrlAsync(string url);
    }
}