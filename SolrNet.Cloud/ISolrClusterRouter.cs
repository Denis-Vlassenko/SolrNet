namespace SolrNet.Cloud {
    public interface ISolrClusterRouter {
        ISolrClusterCore Core { get; }

        string Name { get; }
    }
}