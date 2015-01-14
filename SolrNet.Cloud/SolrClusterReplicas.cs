using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrClusterReplicas : List<ISolrClusterReplica>, ISolrClusterReplicas {
        ISolrClusterReplica ISolrClusterReplicas.this[int index] {
            get { return index >= 0 && index < Count ? this[index] : null; }
        }

        public ISolrClusterShard Shard { get; set; }

        public ISolrClusterReplica First { get; set; }

        public ISolrClusterReplica Leader { get; set; }
    }
}
