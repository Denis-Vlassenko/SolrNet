using System;

namespace SolrNet.Cloud {
    public interface ISolrClusterState : IDisposable {
        ISolrClusterCores Cores { get; }
        bool Initialize();
    }
}
