namespace SolrNet.Cloud {
    public class SolrCloudNode {
        public SolrCloudNode(string collection, bool isActive, bool isLeader, int? rangeEnd, int? rangeStart, string url) {
            Collection = collection;
            IsActive = isActive;
            IsLeader = isLeader;
            RangeEnd = rangeEnd;
            RangeStart = rangeStart;
            Url = url;
        }

        public string Collection { get; private set; }

        public bool IsActive { get; private set; }

        public bool IsLeader { get; private set; }
        
        public int? RangeEnd { get; private set; }
        
        public int? RangeStart { get; private set; }

        public string Url { get; private set; }
    }
}