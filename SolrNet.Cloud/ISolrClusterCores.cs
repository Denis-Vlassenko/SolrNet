using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterCores : IEnumerable<KeyValuePair<string, ISolrClusterCore>> {
        ISolrClusterCore this[string name] { get; }

        ISolrCluster Cluster { get; }

        int Count { get; }

        ISolrClusterCore Default { get; }
    }
}
