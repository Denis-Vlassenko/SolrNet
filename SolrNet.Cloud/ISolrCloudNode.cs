namespace SolrNet.Cloud {
    public interface ISolrCloudNode{
        string Id { get; }

        bool IsActive { get; set; }

        bool IsLeader { get; set; }

        string Url { get; }
    }
}