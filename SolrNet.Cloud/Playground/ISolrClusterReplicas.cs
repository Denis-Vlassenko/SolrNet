using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterReplicas : IEnumerable<ISolrClusterReplica> {
        IEnumerable<ISolrClusterReplica> Active { get; }
        IEnumerable<ISolrClusterReplica> Leaders { get; }
    }
}
