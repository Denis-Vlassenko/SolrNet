using System;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterShard : ISolrClusterShard {
        public SolrClusterShard(ISolrClusterCore core, JProperty json) {
            Core = core;
            Name = json.Name;
            Range = SolrClusterShardRange.Parse((string) json.Value["range"]);
            Replicas = new SolrClusterReplicas(this, json.Value["replicas"] as JObject);
            State = (string)json.Value["state"];
            IsActive = "active".Equals(State, StringComparison.OrdinalIgnoreCase);
        }

        public ISolrClusterCore Core { get; private set; }

        public bool IsActive { get; private set; }

        public string Name { get; private set; }

        public SolrClusterShardRange Range { get; private set; }

        public ISolrClusterReplicas Replicas { get; private set; }

        public string State { get; private set; }
    }
}
