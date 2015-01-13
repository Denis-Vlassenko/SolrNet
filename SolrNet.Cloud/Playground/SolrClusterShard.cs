using System;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud.Playground
{
    public class SolrClusterShard : ISolrClusterShard {
        public SolrClusterShard(JProperty json) {
            Name = json.Name;
            Range = (string) json.Value["range"];
            Replicas = new SolrClusterReplicas(json.Value["replicas"] as JObject);
            State = (string)json.Value["state"];
            IsActive = "active".Equals(State, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsActive { get; private set; }
        public string Name { get; private set; }
        public string Range { get; private set; }
        public ISolrClusterReplicas Replicas { get; private set; }
        public string State { get; private set; }
    }
}
