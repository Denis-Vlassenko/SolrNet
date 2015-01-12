namespace SolrNet.Cloud {
    public interface ISolrClusterCore {
        string Name { get; }
        ISolrClusterShards Shards { get; }
        ISolrClusterRouter Router { get; }
    }
}
