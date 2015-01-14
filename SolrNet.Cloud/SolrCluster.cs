using System;
using System.Linq;
using System.Text;
using ZooKeeperNet;

namespace SolrNet.Cloud {
    public class SolrCluster : ISolrCluster, IWatcher {
        public SolrCluster(string zooKeeperConnection, string collectionName, ISolrClusterBalancer clusterBalancer = null, int maxAttempts = 1) {
            
            this.clusterBalancer = clusterBalancer ?? new SolrClusterRandomBalancer();
            
            
            if (string.IsNullOrEmpty(zooKeeperConnection))
                throw new ArgumentNullException("zooKeeperConnection");

            this.maxAttempts = maxAttempts;
            this.clusterBalancer = clusterBalancer;
            this.zooKeeperConnection = zooKeeperConnection;
            exceptionHandlers = new SolrClusterExceptionHandlers(this);
            operationsProvider = new SolrOperationsProvider();

            syncLock = new object();
        }

        private readonly ISolrClusterBalancer clusterBalancer;

        private bool isDisposed;

        private readonly SolrClusterExceptionHandlers exceptionHandlers;

        private bool isInitialized;

        private readonly int maxAttempts;

        private readonly ISolrOperationsProvider operationsProvider;

        private readonly object syncLock;

        private IZooKeeper zooKeeper;

        private readonly string zooKeeperConnection;

        public ISolrClusterCollections Collections { get; private set; }

        public void Dispose() {
            lock (syncLock)
                if (!isDisposed) {
                    zooKeeper.Dispose();
                    isDisposed = true;
                }
        }

        public ISolrOperations<T> GetOperations<T>(string collectionName = null, int? routingHash = null) {
            if (!isInitialized)
                throw new InvalidOperationException("This object was not initialized yet.");
            var shard = Route(collectionName, routingHash);
            if (shard == null)
                throw new ApplicationException("No appropriate replica was found.");
            return new SolrClusterOperations<T>(clusterBalancer, exceptionHandlers, maxAttempts, operationsProvider, shard.Replicas);
        }

        public bool Initialize() {
            lock (syncLock)
                if (!isInitialized) {
                    isInitialized = Update();
                }
            return isInitialized;
        }

        void IWatcher.Process(WatchedEvent @event) {
            if (@event.Type == EventType.NodeDataChanged)
                lock (syncLock) 
                    Update();
        }

        private ISolrClusterShard Route(string collectionName = null, int? routingHash = null)
        {
            var collection = collectionName == null ? Collections[0] : Collections[collectionName];
            return routingHash.HasValue
                ? collection.Shards.FirstOrDefault(
                    shard => shard.Range.Start <= routingHash && shard.Range.End >= routingHash)
                : collection.Shards[0];
        }

        private bool Update() {
            try {
                if (zooKeeper == null)
                    zooKeeper = new ZooKeeper(zooKeeperConnection, TimeSpan.FromSeconds(10), this);
                Collections = SolrClusterStateParser.ParseJson(
                    Encoding.Default.GetString(
                        zooKeeper.GetData("/clusterstate.json", true, null)));
                return true;
            } catch (Exception exception) {
                exceptionHandlers.Handle(exception);
            }
            return false;
        }

        public event EventHandler<SolrClusterExceptionEventArgs> Exception {
            add { exceptionHandlers.Add(value); }
            remove { exceptionHandlers.Remove(value); }
        }
    }
}