namespace SolrNet.Cloud {
    public interface ISolrClusterShard {
        bool IsActive { get; }

        string Name { get; }

        ISolrClusterShardRange Range { get; }

        ISolrClusterReplicas Replicas { get; }

        string State { get; }
    }
}