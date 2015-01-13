using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterShards : IEnumerable<ISolrClusterShard> {
        ISolrClusterShard this[string range] { get; }
    }
}
