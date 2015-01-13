using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Org.Apache.Zookeeper.Data;
using SolrNet.Cloud.ZkInternal;
using ZooKeeperNet;

namespace SolrNet.Cloud
{
    class watcher : IWatcher
    {
        private readonly ManualResetEventSlim _connected = new ManualResetEventSlim(false);
        private WatchedEvent _event;

        public void WaitUntilConnected(int timeoutSecs) {
            _connected.Wait(TimeSpan.FromSeconds(timeoutSecs));

            if (_event == null) throw new ApplicationException("bad state");
            if (_event.State != KeeperState.SyncConnected)
                throw new ApplicationException("cannot connect");
        }

        public void Process(WatchedEvent @event)
        {
            _event = @event;
            _connected.Set();
        }
    }
    
    public class ZkStateReader : IClusterStateReader
    {
        public CollectionClusterState GetClusterState(string zkHost, string collectionName) {
            
            var w = new watcher(); 
            var zk = new ZooKeeper(zkHost, TimeSpan.FromSeconds(1), w);
            w.WaitUntilConnected(1);

            //var nodes = zk.GetChildren("/live_nodes", false).ToList();
            
            var clusterState = zk.GetData("/clusterstate.json", false, null);
            var json = Encoding.Default.GetString(clusterState);

            ClusterState state = ClusterState.FromJson(json);

            return new CollectionClusterState(state.Collections[collectionName]);
        }

        public void UpdateClusterState() {
            // go to zk and cache clusterstate
        }

        public void Subscribe() {
            var w = new watcher();
            var zk = new ZooKeeper("", TimeSpan.FromSeconds(1), w);
            w.WaitUntilConnected(1);

            zk.GetData("", w, new Stat());


        }
    }

    public interface IClusterStateReader {
        CollectionClusterState GetClusterState(string zkHost, string collectionName);
        void UpdateClusterState();
        void Subscribe();
    }
    
}
