namespace SolrNet.Cloud {
    public class SolrClusterReplica : ISolrClusterReplica {
        public string BaseUrl { get; set; }

        public bool IsActive { get; set; }

        public bool IsLeader { get; set; }

        public string Name { get; set; }

        public string NodeName { get; set; }

        public ISolrClusterShard Shard { get; set; }

        public string State { get; set; }

        public string Url { get; set; }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}