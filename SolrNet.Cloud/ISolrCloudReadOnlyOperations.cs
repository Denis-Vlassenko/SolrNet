namespace SolrNet.Cloud {
    public interface ISolrCloudReadOnlyOperations<T> : ISolrReadOnlyOperations<T>, ISolrCloudBasicReadOnlyOperations<T> {
    }
}