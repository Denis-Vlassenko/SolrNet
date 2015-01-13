using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SolrNet.Cloud.ZkInternal;

namespace SolrNet.Cloud
{
    public class ClusterState {
        public static ClusterState FromJson(string json) {
            return new ClusterState() {Collections = JsonConvert.DeserializeObject<Dictionary<string, ZkCollection>>(json)};
        }

        public Dictionary<string, ZkCollection> Collections { get; set; }
    }

    public class CollectionClusterState {
        
        public CollectionClusterState(ZkCollection collection) {
            Collection = collection;
        }
       
        public ZkCollection Collection;

        public IList<string> ActiveUrls {
            get {
                return Collection.shards.Values.SelectMany(
                    shard => shard.replicas.Values
                        .Where(r=>r.state=="active"))
                        .Select(f => f.base_url + "/" + f.core).ToList();
            }
        }

        public IList<CoreNode> Leaders {
            get {
                return Collection.shards.Values.SelectMany(
                        shard => shard.replicas.Values
                            .Where(r => (r.state == "active" && r.leader == "true"))).ToList();
            }
        }

        // convert to Urls
        public IList<string> LeadersUrls
        {
            get {
                return 
                    Leaders.Select(r => r.base_url != null ? r.base_url + "/" + r.core : null).ToList();
            }
        }

    }

    namespace ZkInternal {

        public class CoreNode {
            public string core { get; set; }
            public string base_url { get; set; }
            public string node_name { get; set; }
            public string state { get; set; }
            public string leader { get; set; }
        }

        public class Shard {
            public string range { get; set; }
            public string state { get; set; }
            public Dictionary<string, CoreNode> replicas { get; set; }
        }

        public class Router {
            public string name { get; set; }
        }

        public class ZkCollection {
            public string replicationFactor { get; set; }
            public Dictionary<string, Shard> shards { get; set; }
            public Router router { get; set; }
            public string maxShardsPerNode { get; set; }
            public string autoAddReplicas { get; set; }
            public string autoCreated { get; set; }
        }
    }
}
