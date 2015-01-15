using System.Threading;

namespace SolrNet.Cloud {
    public class SolrClusterRoundRobinBalancer : SolrClusterBalancerBase {
        public SolrClusterRoundRobinBalancer() {
            cursor = int.MinValue;
        }

        private int cursor;

        protected override int GetNextIndex(int replicasCount) {
            return Interlocked.Increment(ref cursor);
        }
    }
}