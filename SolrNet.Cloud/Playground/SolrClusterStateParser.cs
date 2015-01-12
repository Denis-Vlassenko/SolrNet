using System;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public static class SolrClusterStateParser {
        public static bool IsActive(string state) {
            return "active".Equals(state, StringComparison.OrdinalIgnoreCase);
        }

        public static ISolrClusterCores ParseJson(string json) {
            return new SolrClusterCores(JObject.Parse(json));
        }
    }
}