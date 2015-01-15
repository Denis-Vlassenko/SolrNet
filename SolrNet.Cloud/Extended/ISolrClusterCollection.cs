namespace SolrNet.Cloud {
    public interface ISolrClusterCollection {
        string Name { get; }

        ISolrClusterShards Shards { get; }

        ISolrClusterRouter Router { get; }
    }
}
