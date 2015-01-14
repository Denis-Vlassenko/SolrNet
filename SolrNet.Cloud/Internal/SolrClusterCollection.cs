namespace SolrNet.Cloud {
    internal class SolrClusterCollection : ISolrClusterCollection {
        public string Name { get; set; }

        public ISolrClusterRouter Router { get; set; }

        public ISolrClusterShards Shards { get; set; }
    }
}
