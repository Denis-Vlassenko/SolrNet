using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterShards : IEnumerable<ISolrClusterShard> {
        ISolrClusterShard this[int index] { get; }

        int Count { get; }
    }
}
