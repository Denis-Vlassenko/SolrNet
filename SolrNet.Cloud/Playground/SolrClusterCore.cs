using Newtonsoft.Json;

namespace SolrNet.Cloud
{
    public class SolrClusterCore : ISolrClusterCore {
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("shards")]
        public ISolrClusterShards Shards { get; private set; }
        [JsonProperty("router")]
        public ISolrClusterRouter Router { get; private set; }
    }
}
