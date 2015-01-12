namespace SolrNet.Cloud
{
    public interface ISolrClusterShard {
        string Name { get; }
        string Range { get; }
        ISolrClusterReplicas Replicas { get; }
    }
}