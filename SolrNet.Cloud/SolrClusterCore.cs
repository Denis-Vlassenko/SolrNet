using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterCore : ISolrClusterCore {
        public SolrClusterCore(ISolrCluster cluster, JProperty json) {
            Cluster = cluster;
            Name = json.Name;
            Router = new SolrClusterRouter(this, json.Value["router"] as JObject);
            Shards = new SolrClusterShards(this, json.Value["shards"] as JObject);
        }

        public string Name { get; private set; }

        public ISolrCluster Cluster { get; private set; }

        public ISolrClusterRouter Router { get; private set; }

        public ISolrClusterShards Shards { get; private set; }
    }
}
