using System;
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Cloud.Playground
{
    public class SolrClusterRandomBalancer : SolrClusterBalancerBase {
        public SolrClusterRandomBalancer(ISolrOperationsProvider provider) : base(provider) {
            this.random = new Random();
        }

        private readonly Random random;

        protected override ISolrClusterReplica SelectReplica(IEnumerable<ISolrClusterReplica> replicas) {
            var array = replicas.ToArray();
            lock (random)
                return array[random.Next(array.Length)];
        }
    }
}
