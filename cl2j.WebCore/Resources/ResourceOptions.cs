namespace cl2j.WebCore.Resources
{
    public class ResourceOptions
    {
        public string DataStoreName { get; set; } = null!;
        public IList<string> Languages { get; set; } = new List<string>();
        public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(30 * 60);
    }
}