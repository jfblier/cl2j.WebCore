using System;

namespace cl2j.WebCore.Routes
{
    public class RouteOptions
    {
        public RouteOptions()
        {
            RefreshInterval = TimeSpan.FromSeconds(30 * 60);
        }

        public string DataStoreName { get; set; }
        public TimeSpan RefreshInterval { get; set; }
    }
}