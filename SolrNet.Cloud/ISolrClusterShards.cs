using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterShards : IEnumerable<KeyValuePair<string, ISolrClusterShard>> {
        ISolrClusterShard this[string name] { get; }

        ISolrClusterShard this[int hash] { get; }

        ISolrClusterCore Core { get; }

        int Count { get; }

        ISolrClusterShard Default { get; }
    }
}
