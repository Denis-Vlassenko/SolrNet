namespace SolrNet.Cloud {
    public interface ISolrClusterBalancer {
        ISolrClusterReplica Balance(ISolrClusterReplicas replicas, bool leader);
    }
}