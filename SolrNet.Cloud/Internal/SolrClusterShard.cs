namespace SolrNet.Cloud {
    internal class SolrClusterShard : ISolrClusterShard {
        public bool IsActive { get; set; }

        public string Name { get; set; }

        public ISolrClusterShardRange Range { get; set; }

        public ISolrClusterReplicas Replicas { get; set; }

        public string State { get; set; }
    }
}
