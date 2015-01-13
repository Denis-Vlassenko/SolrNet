using Newtonsoft.Json.Linq;
using SolrNet.Cloud.Playground;

namespace SolrNet.Cloud
{
    public class SolrClusterCore : ISolrClusterCore {
        public SolrClusterCore(JProperty json) {
            Name = json.Name;
            Router = new SolrClusterRouter(json.Value["router"] as JObject);
            Shards = new SolrClusterShards(json.Value["shards"] as JObject);
        }
        public string Name { get; private set; }
        public ISolrClusterRouter Router { get; private set; }
        public ISolrClusterShards Shards { get; private set; }
    }
}
