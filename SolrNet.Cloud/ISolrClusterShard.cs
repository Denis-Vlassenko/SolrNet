namespace SolrNet.Cloud {
    public interface ISolrClusterShard {
        ISolrClusterCore Core { get; }

        bool IsActive { get; }

        string Name { get; }

        SolrClusterShardRange Range { get; }

        ISolrClusterReplicas Replicas { get; }

        string State { get; }
    }
}