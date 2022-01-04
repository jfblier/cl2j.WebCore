using cl2j.Tooling;
using System.Collections.Generic;

namespace cl2j.WebCore.Resources
{
    public class ResourceCollection
    {
        private readonly IDictionary<string, Localized<string>> values;

        public ResourceCollection(IDictionary<string, Localized<string>> resources)
        {
            values = resources;
        }

        public Localized<string> Get(string key)
        {
            if (!values.TryGetValue(key, out var localizeString))
                return null;
            return localizeString;
        }

        public string Get(string key, string language, params object[] parameters)
        {
            if (!values.TryGetValue(key, out var localizeString))
                return $"{{{language}:{key}}}";

            string value = localizeString[language];

            if (parameters.Length > 0)
                value = string.Format(value, parameters);

            return value;
        }

        public IDictionary<string, string> GetAll(string language)
        {
            var result = new Dictionary<string, string>();
            foreach (var key in values.Keys)
                result[key] = values[key][language];
            return result;
        }
    }
}