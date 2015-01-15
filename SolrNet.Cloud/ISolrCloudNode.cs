namespace SolrNet.Cloud {
    public interface ISolrCloudNode {
        string Collection { get; }

        bool IsActive { get; }

        bool IsLeader { get; }

        int? RangeEnd { get; }

        int? RangeStart { get; }

        string Url { get; }
    }
}