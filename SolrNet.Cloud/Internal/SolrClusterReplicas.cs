using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Cloud {
    internal class SolrClusterReplicas : ISolrClusterReplicas {
        public SolrClusterReplicas(IEnumerable<ISolrClusterReplica> replicas) {
            list = replicas.ToList();
            Leader = list.FirstOrDefault(replica => replica.IsLeader);
        }

        private readonly List<ISolrClusterReplica> list;

        ISolrClusterReplica ISolrClusterReplicas.this[int index] {
            get {
                return index >= 0 && index < list.Count
                    ? list[index]
                    : null;
            }
        }

        public int Count {
            get { return list.Count; }
        }

        public ISolrClusterReplica Leader { get; private set; }

        public IEnumerator<ISolrClusterReplica> GetEnumerator() {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
