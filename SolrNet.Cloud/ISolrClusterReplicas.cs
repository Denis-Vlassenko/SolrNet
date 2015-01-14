using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterReplicas : IEnumerable<ISolrClusterReplica> {
        ISolrClusterReplica this[int index] { get; }

        int Count { get; }

        ISolrClusterReplica First { get; }

        ISolrClusterReplica Leader { get; }

        ISolrClusterShard Shard { get; }
    }
}
