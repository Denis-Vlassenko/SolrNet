using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterCollections : IEnumerable<KeyValuePair<string, ISolrClusterCollection>> {
        ISolrClusterCollection this[string name] { get; }

        ISolrCluster Cluster { get; }

        int Count { get; }

        ISolrClusterCollection First { get; }
    }
}
