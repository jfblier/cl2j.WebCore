using cl2j.Tooling;

namespace cl2j.WebCore.Routes
{
    public class Route
    {
        public string Id { get; set; }
        public Localized<string> Pattern { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public override string ToString()
        {
            return $"{Pattern} --> {Controller}.{Action}";
        }
    }
}