namespace SolrNet.Cloud
{
    public interface ISolrClusterShard {
        bool IsActive { get; }
        string Name { get; }
        string Range { get; }
        ISolrClusterReplicas Replicas { get; }
        string State { get; }
    }
}