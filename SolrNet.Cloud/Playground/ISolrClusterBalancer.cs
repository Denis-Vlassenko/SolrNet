using System;

namespace SolrNet.Cloud
{
    public interface ISolrClusterBalancer {
        TResult Balance<T, TResult>(Func<ISolrOperations<T>, TResult> operation, ISolrClusterReplicas replicas, bool write);
    }
}