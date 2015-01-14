using System;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterReplica : ISolrClusterReplica {
        public SolrClusterReplica(ISolrClusterShard shard, JProperty json) {
            BaseUrl = (string) json.Value["base_url"];
            var leader = json.Value["leader"];
            IsLeader = leader != null && (bool) leader;
            Name = json.Name;
            NodeName = (string) json.Value["node_name"];
            Shard = shard;
            State = (string) json.Value["state"];
            IsActive = "active".Equals(State, StringComparison.OrdinalIgnoreCase);
            Url = BaseUrl + @"\" + Shard.Core.Name;
        }

        public string BaseUrl { get; private set; }

        public bool IsActive { get; private set; }

        public bool IsLeader { get; private set; }

        public string Name { get; private set; }

        public string NodeName { get; private set; }

        public ISolrClusterShard Shard { get; private set; }

        public string State { get; private set; }

        public string Url { get; private set; }

        public void Deactivate() {
            IsActive = false;
        }
    }
}