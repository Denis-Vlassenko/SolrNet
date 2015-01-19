namespace SolrNet.Cloud {
    public class SolrCloudReplica {
        public SolrCloudReplica(bool isActive, bool isLeader, string name, string url) {
            IsActive = isActive;
            IsLeader = isLeader;
            Name = name;
            Url = url;
        }

        public bool IsActive { get; private set; }

        public bool IsLeader { get; private set; }

        public string Name { get; private set; }

        public string Url { get; private set; }
    }
}