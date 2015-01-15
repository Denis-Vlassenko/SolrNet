using System;

namespace SolrNet.Cloud {
    public interface ISolrCluster : IDisposable {
        ISolrClusterCollections Collections { get; }

        ISolrOperations<T> GetOperations<T>(string collectionName = null, int? routingHash = null);

        bool Initialize();

        event EventHandler<SolrClusterExceptionEventArgs> Exception;
    }
}
