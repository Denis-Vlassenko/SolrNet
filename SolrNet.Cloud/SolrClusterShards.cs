using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrClusterShards : Dictionary<string, ISolrClusterShard>, ISolrClusterShards {
        public SolrClusterShards() : base(StringComparer.OrdinalIgnoreCase) { }

        ISolrClusterShard ISolrClusterShards.this[string name] {
            get {
                if (name == null)
                    return First;
                ISolrClusterShard shard;
                TryGetValue(name, out shard);
                return shard;
            }
        }

        ISolrClusterShard ISolrClusterShards.this[int hash] {
            get {
                foreach (var shard in Values)
                    if (shard.Range.Start <= hash && shard.Range.End >= hash)
                        return shard;
                return null;
            }
        }

        public ISolrClusterCollection Collection { get; set; }

        public ISolrClusterShard First { get; set; }
    }
}
