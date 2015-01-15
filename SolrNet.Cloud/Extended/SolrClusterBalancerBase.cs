using System.Collections.Generic;

namespace SolrNet.Cloud
{
    public abstract class SolrClusterBalancerBase : ISolrClusterBalancer {
        public ISolrClusterReplica Balance(ISolrClusterReplicas replicas, bool leader) {
            if (leader)
                return replicas.Leader.IsActive
                    ? replicas.Leader
                    : null;
            var probes = new HashSet<int>();
            while (probes.Count < replicas.Count)
            {
                var index = GetNextIndex(replicas.Count) % replicas.Count;
                if (!probes.Add(index))
                    continue;
                var replica = replicas[index];
                if (replica.IsActive)
                    return replica;
            }
            return null;
        }

        protected abstract int GetNextIndex(int replicasCount);
    }
}
