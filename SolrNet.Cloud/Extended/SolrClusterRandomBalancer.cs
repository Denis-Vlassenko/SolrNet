using System;

namespace SolrNet.Cloud {
    public class SolrClusterRandomBalancer : SolrClusterBalancerBase {
        public SolrClusterRandomBalancer() {
            random = new Random();
        }

        private readonly Random random;

        protected override int GetNextIndex(int replicasCount) {
            lock (random)
                return random.Next(replicasCount);
        }
    }
}