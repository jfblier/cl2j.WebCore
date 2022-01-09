using cl2j.Tooling;

namespace cl2j.WebCore.Routes
{
    public class Route
    {
        public string Id { get; set; } = null!;
        public Localized<string> Pattern { get; set; } = null!;
        public string Controller { get; set; } = null!;
        public string Action { get; set; } = null!;

        public override string ToString()
        {
            return $"{Pattern} --> {Controller}.{Action}";
        }
    }
}