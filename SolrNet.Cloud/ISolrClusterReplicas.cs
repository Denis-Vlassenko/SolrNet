using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterReplicas : IEnumerable<ISolrClusterReplica> {
        ISolrClusterReplica this[int index] { get; }

        ISolrClusterShard Shard { get; }

        int Count { get; }

        ISolrClusterReplica Default { get; }

        ISolrClusterReplica Leader { get; }
    }
}
