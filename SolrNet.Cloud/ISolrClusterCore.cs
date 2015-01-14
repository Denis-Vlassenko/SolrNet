namespace SolrNet.Cloud {
    public interface ISolrClusterCore {
        string Name { get; }

        ISolrCluster Cluster { get; }

        ISolrClusterShards Shards { get; }

        ISolrClusterRouter Router { get; }
    }
}
