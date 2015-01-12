using System;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud.Playground {
    public class SolrClusterReplica : ISolrClusterReplica {
        public SolrClusterReplica(JProperty json) {
            BaseUrl = (string) json.Value["base_url"];
            var leader = json.Value["leader"];
            IsLeader = leader != null && (bool) leader;
            Name = json.Name;
            NodeName = (string) json.Value["node_name"];
            State = (string) json.Value["state"];
            IsActive = SolrClusterStateParser.IsActive(State);
        }

        public string BaseUrl { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsLeader { get; private set; }
        public string Name { get; private set; }
        public string NodeName { get; private set; }
        public string State { get; private set; }
    }
}