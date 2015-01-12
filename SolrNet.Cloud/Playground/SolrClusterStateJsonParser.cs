using Newtonsoft.Json;

namespace SolrNet.Cloud {
    public static class SolrClusterStateJsonParser {
        public static ISolrClusterCores Parse(string state) {
            return JsonConvert.DeserializeObject<ISolrClusterCores>(state);
        }
    }
}