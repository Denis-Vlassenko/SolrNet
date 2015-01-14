using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrClusterRandomBalancer : ISolrClusterBalancer {
        public ISolrClusterReplica Balance(ISolrClusterReplicas replicas, bool leader) {
            if (leader)
                return replicas.Leader;
            var probes = new HashSet<int>();
            var random = new Random();
            while (probes.Count < replicas.Count)
            {
                var index = random.Next(replicas.Count);
                if (!probes.Add(index))
                    continue;
                var replica = replicas[index];
                if (replica.IsActive)
                    return replica;
            }
            return null;
        }
    }
}