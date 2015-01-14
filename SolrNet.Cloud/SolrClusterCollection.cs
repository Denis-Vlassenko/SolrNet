using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrClusterCollection : ISolrClusterCollection {
        public string Name { get; set; }

        public ISolrCluster Cluster { get; set; }

        public ISolrClusterRouter Router { get; set; }

        public ISolrClusterShards Shards { get; set; }
    }
}
