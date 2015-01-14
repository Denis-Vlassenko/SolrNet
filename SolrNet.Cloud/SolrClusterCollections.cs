using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrClusterCollections : Dictionary<string, ISolrClusterCollection>, ISolrClusterCollections {
        public SolrClusterCollections() : base(StringComparer.OrdinalIgnoreCase) {}

        ISolrClusterCollection ISolrClusterCollections.this[string name] {
            get {
                if (name == null)
                    return First;
                ISolrClusterCollection core;
                TryGetValue(name, out core);
                return core;
            }
        }

        public ISolrCluster Cluster { get; set; }

        public ISolrClusterCollection First { get; set; }
    }
}