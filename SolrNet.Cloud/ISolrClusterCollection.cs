using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterCollection {
        string Name { get; }

        ISolrCluster Cluster { get; }

        ISolrClusterShards Shards { get; }

        ISolrClusterRouter Router { get; }
    }
}
