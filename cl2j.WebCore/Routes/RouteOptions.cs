namespace cl2j.WebCore.Routes
{
    public class RouteOptions
    {
        public string DataStoreName { get; set; } = null!;
        public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(30 * 60);
    }
}