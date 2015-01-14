using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterReplicas : List<ISolrClusterReplica>, ISolrClusterReplicas {
        public SolrClusterReplicas(ISolrClusterShard shard, JObject json) {
            Shard = shard;
            foreach (var property in json.Properties()) {
                var replica = new SolrClusterReplica(shard, property);
                if (Count == 0)
                    Default = replica;
                if (replica.IsLeader)
                    Leader = replica;
                Add(replica);
            }
        }

        ISolrClusterReplica ISolrClusterReplicas.this[int index] {
            get { return index >= 0 && index < Count ? this[index] : null; }
        }

        public ISolrClusterShard Shard { get; private set; }

        public ISolrClusterReplica Default { get; private set; }

        public ISolrClusterReplica Leader { get; private set; }
    }
}
