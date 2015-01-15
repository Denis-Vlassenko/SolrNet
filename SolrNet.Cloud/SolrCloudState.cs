using System;
using System.Collections.Generic;
using System.Text;
using ZooKeeperNet;

namespace SolrNet.Cloud {
    public class SolrCloudState : ISolrCloudState, IWatcher {
        public SolrCloudState(string zooKeeperConnection)
        {
            if (string.IsNullOrEmpty(zooKeeperConnection))
                throw new ArgumentNullException("zooKeeperConnection");
            this.zooKeeperConnection = zooKeeperConnection;
            syncLock = new object();
        }

        private bool isDisposed;

        private bool isInitialized;

        private readonly object syncLock;

        private IZooKeeper zooKeeper;

        private readonly string zooKeeperConnection;

        public IEnumerable<SolrCloudNode> Nodes { get; private set; }

        public void Dispose() {
            lock (syncLock)
                if (!isDisposed) {
                    zooKeeper.Dispose();
                    isDisposed = true;
                }
        }

        public void Init() {
            lock (syncLock)
                if (!isInitialized) {
                    Update();
                    isInitialized = true;
                }
        }

        void IWatcher.Process(WatchedEvent @event) {
            if (@event.Type == EventType.NodeDataChanged)
                lock (syncLock)
                    try {
                        Update();
                    } catch {}
        }

        private void Update() {
            if (zooKeeper == null)
                zooKeeper = new ZooKeeper(zooKeeperConnection, TimeSpan.FromSeconds(10), this);
            Nodes = SolrCloudStateParser.ParseJsonToNodes(
                Encoding.Default.GetString(
                    zooKeeper.GetData("/clusterstate.json", true, null)));
        }
    }
}