using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Cloud {
    internal class SolrClusterShards : ISolrClusterShards {
        public SolrClusterShards(IEnumerable<ISolrClusterShard> shards) {
            list = shards.ToList();
        }

        private readonly List<ISolrClusterShard> list;
        
        ISolrClusterShard ISolrClusterShards.this[int index] {
            get {
                return index >= 0 && index < list.Count
                    ? list[index]
                    : null;
            }
        }

        public int Count {
            get { return list.Count; }
        }

        public IEnumerator<ISolrClusterShard> GetEnumerator() {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
