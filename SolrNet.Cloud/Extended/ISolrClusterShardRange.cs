namespace SolrNet.Cloud {
    public interface ISolrClusterShardRange {
        int End { get; }
        int Start { get; }
    }
}