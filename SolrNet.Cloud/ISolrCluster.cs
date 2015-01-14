using System;

namespace SolrNet.Cloud {
    public interface ISolrCluster : IDisposable {
        ISolrClusterCores Cores { get; }

        ISolrOperations<T> GetOperations<T>(string coreName, int? routingHash = null);

        ISolrOperations<T> GetOperations<T>(string coreName = null, string shardName = null);

        bool Initialize();

        event EventHandler<SolrClusterExceptionEventArgs> Exception;
    }
}
