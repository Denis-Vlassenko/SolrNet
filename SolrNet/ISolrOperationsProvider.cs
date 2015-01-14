namespace SolrNet
{
    public interface ISolrOperationsProvider {
        ISolrBasicOperations<T> GetBasicOperations<T>(ISolrConnection connection);

        ISolrBasicOperations<T> GetBasicOperations<T>(string url);

        ISolrOperations<T> GetOperations<T>(ISolrConnection connection);

        ISolrOperations<T> GetOperations<T>(string url);
    }
}