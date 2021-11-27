using System;
using System.Collections.Generic;

namespace cl2j.WebCore.Resources
{
    public class ResourceOptions
    {
        public ResourceOptions()
        {
            RefreshInterval = TimeSpan.FromSeconds(30 * 60);
        }

        public string DataStoreName { get; set; }
        public IList<string> Languages { get; set; } = new List<string>();
        public TimeSpan RefreshInterval { get; set; }
    }
}