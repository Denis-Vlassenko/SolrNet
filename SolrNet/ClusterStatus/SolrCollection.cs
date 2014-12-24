using System.Collections.Generic;

namespace SolrNet.ClusterStatus {
    public class SolrCollection {
        public SolrCollection() {
            SolrShards = new List<SolrShard>();
        }

        public IList<string> Aliases { get; set; }

        /// <summary>
        /// Collection name
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        public IList<SolrShard> SolrShards { get; set; }
    }
}
