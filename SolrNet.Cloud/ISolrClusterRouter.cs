namespace SolrNet.Cloud {
    public interface ISolrClusterRouter {
        ISolrClusterCollection Collection { get; }

        string Name { get; }
    }
}