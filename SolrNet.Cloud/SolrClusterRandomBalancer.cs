using System;
using System.Collections.Generic;

namespace SolrNet.Cloud.Playground
{
    public class SolrClusterRandomBalancer : SolrClusterBalancerBase {
        public SolrClusterRandomBalancer(ISolrOperationsProvider provider) : base(provider) {
            random = new Random();
        }

        private readonly Random random;

        protected override ISolrClusterReplica SelectReplica(ISolrClusterReplicas replicas) {
            var probes = new HashSet<int>();
            while (probes.Count < replicas.Count) {
                int index;
                lock (random)
                    index = random.Next(replicas.Count);
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