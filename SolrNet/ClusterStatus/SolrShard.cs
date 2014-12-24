using System.Collections.Generic;

namespace SolrNet.ClusterStatus {
    public class SolrShard {
        public SolrShard() {
            SolrReplicas = new List<SolrReplica>();
        }

        public string Name { get; set; }

        public IList<SolrReplica> SolrReplicas { get; set; }

        public string State { get; set; }
    }
}
