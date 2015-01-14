using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterCores : Dictionary<string, ISolrClusterCore>, ISolrClusterCores {
        private SolrClusterCores(ISolrCluster cluster, JObject json) : base(StringComparer.OrdinalIgnoreCase) {
            Cluster = cluster;
            foreach (var property in json.Properties()) {
                var core = new SolrClusterCore(cluster, property);
                if (Count == 0)
                    Default = core;
                Add(core.Name, core);
            }
        }

        ISolrClusterCore ISolrClusterCores.this[string name] {
            get {
                if (name == null)
                    return Default;
                ISolrClusterCore core;
                TryGetValue(name, out core);
                return core;
            }
        }

        public ISolrCluster Cluster { get; private set; }

        public ISolrClusterCore Default { get; private set; }

        public static ISolrClusterCores Create(ISolrCluster cluster, string json) {
            return new SolrClusterCores(cluster, JObject.Parse(json));
        }
    }
}