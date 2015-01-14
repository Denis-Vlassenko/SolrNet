using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterShards : IEnumerable<KeyValuePair<string, ISolrClusterShard>> {
        ISolrClusterShard this[string name] { get; }

        ISolrClusterShard this[int hash] { get; }

        ISolrClusterCollection Collection { get; }

        int Count { get; }

        ISolrClusterShard First { get; }
    }
}
