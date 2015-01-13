using System;

namespace SolrNet.Cloud {
    public interface ISolrCluster : IDisposable {
        ISolrClusterCores Cores { get; }

        ISolrOperations<T> GetOperations<T>(string coreName, string shardRange = null);

        bool Initialize();

        event EventHandler<SolrClusterExceptionEventArgs> Exception;
    }
}
