using System;
using System.Text;
using ZooKeeperNet;

namespace SolrNet.Cloud.ZooKeeperClient {
    public class SolrCloudStateProvider : ISolrCloudStateProvider, IWatcher {
        public SolrCloudStateProvider(string zooKeeperConnection)
        {
            if (string.IsNullOrEmpty(zooKeeperConnection))
                throw new ArgumentNullException("zooKeeperConnection");
            this.zooKeeperConnection = zooKeeperConnection;
            syncLock = new object();
        }

        private bool isDisposed;

        private bool isInitialized;

        private SolrCloudState state;

        private readonly object syncLock;

        private IZooKeeper zooKeeper;

        private readonly string zooKeeperConnection;

        public void Dispose()
        {
            lock (syncLock)
                if (!isDisposed)
                {
                    zooKeeper.Dispose();
                    isDisposed = true;
                }
        }

        public SolrCloudState GetCloudState() {
            return state;
        }

        public void Init()
        {
            lock (syncLock)
                if (!isInitialized)
                {
                    Update();
                    isInitialized = true;
                }
        }

        void IWatcher.Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeDataChanged)
                lock (syncLock)
                    try
                    {
                        Update();
                    }
                    catch { }
        }

        private void Update()
        {
            if (zooKeeper == null)
                zooKeeper = new ZooKeeper(
                    zooKeeperConnection, 
                    TimeSpan.FromSeconds(10), this);
            state = SolrCloudStateParser.Parse(
                Encoding.Default.GetString(
                    zooKeeper.GetData("/clusterstate.json", true, null)));
        }
    }
}
