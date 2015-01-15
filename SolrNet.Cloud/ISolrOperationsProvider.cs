namespace SolrNet
{
    public interface ISolrOperationsProvider {
        ISolrBasicOperations<T> GetBasicOperations<T>(string url);

        ISolrOperations<T> GetOperations<T>(string url);
    }
}