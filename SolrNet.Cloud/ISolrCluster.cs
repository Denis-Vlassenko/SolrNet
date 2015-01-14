using System;

namespace SolrNet.Cloud {
    public interface ISolrCluster : IDisposable {
        ISolrClusterCollections Collections { get; }

        ISolrOperations<T> GetOperations<T>(string coreName, int routingHash);

        ISolrOperations<T> GetOperations<T>(string coreName = null, string shardName = null);

        bool Initialize();

        event EventHandler<SolrClusterExceptionEventArgs> Exception;
    }
}
