namespace SolrNet.Cloud {
    public class SolrClusterShard : ISolrClusterShard {
        public ISolrClusterCollection Collection { get; set; }

        public bool IsActive { get; set; }

        public string Name { get; set; }

        public SolrClusterShardRange Range { get; set; }

        public ISolrClusterReplicas Replicas { get; set; }

        public string State { get; set; }
    }
}
