namespace SolrNet.Cloud {
    public interface ISolrCloudBasicOperations<T> : ISolrBasicOperations<T>, ISolrCloudBasicReadOnlyOperations<T> {
    }
}