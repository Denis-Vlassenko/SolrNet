namespace SolrNet.Cloud {
    public interface ISolrClusterReplica {
        string BaseUrl { get; }

        bool IsActive { get; }

        bool IsLeader { get; }

        string Name { get; }

        string NodeName { get; }

        ISolrClusterShard Shard { get; }

        string State { get; }

        string Url { get; }

        void Deactivate();
    }
}