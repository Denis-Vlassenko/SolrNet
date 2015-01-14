using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterCollections : IEnumerable<ISolrClusterCollection> {
        ISolrClusterCollection this[int index] { get; }

        ISolrClusterCollection this[string name] { get; }

        int Count { get; }
    }
}
