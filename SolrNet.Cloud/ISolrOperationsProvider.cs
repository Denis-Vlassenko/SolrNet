namespace SolrNet.Cloud.Playground
{
    public interface ISolrOperationsProvider {
        ISolrOperations<T> GetOperations<T>(ISolrClusterReplica replica);
    }
}