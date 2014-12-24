namespace SolrNet.Cloud
{
    public interface IOperationsResolver {
        ISolrOperations<T> Resolve<T>(string id);
    }
}
