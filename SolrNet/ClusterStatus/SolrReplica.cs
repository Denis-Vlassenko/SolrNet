namespace SolrNet.ClusterStatus {
    public class SolrReplica
    {
        public string BaseUrl { get; set; }

        public string Core { get; set; }

        public bool IsActive { get; set; }

        public bool IsLeader { get; set; }

        public string Name { get; set; }

        public string NodeName { get; set; }

        public string State { get; set; }

        public string Url { get; set; }
    }
}