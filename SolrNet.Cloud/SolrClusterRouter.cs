namespace SolrNet.Cloud {
    public class SolrClusterRouter : ISolrClusterRouter {
        public ISolrClusterCollection Collection { get; set; }

        public string Name { get; set; }
    }
}
