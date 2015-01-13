using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud.Playground
{
    public class SolrClusterReplicas : List<ISolrClusterReplica>, ISolrClusterReplicas
    {
        public SolrClusterReplicas(JObject json) {
            foreach (var property in json.Properties()) {
                var replica = new SolrClusterReplica(property);
                if (replica.IsLeader)
                    Leader = replica;
                Add(replica);
            }
        }

        public ISolrClusterReplica Leader { get; private set; }
    }
}
