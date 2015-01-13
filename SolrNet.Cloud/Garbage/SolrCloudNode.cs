namespace SolrNet.Cloud {
    public class SolrCloudNode : ISolrCloudNode {
        public SolrCloudNode(string id, string url) {
            this.Id = id;
            this.IsActive = true;
            this.IsLeader = false;
            this.Url = url;
        }

        public bool IsActive { get; set; }

        public bool IsLeader { get; set; }

        public string Id { get; private set; }

        public string Url { get; private set; }
    }
}