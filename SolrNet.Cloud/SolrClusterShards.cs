using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterShards : Dictionary<string, ISolrClusterShard>, ISolrClusterShards {
        public SolrClusterShards(ISolrClusterCore core, JObject json) : base(StringComparer.OrdinalIgnoreCase) {
            Core = core;
            foreach (var property in json.Properties()) {
                var shard = new SolrClusterShard(core, property);
                if (Count == 0)
                    Default = shard;
                Add(shard.Name, shard);
            }
        }

        ISolrClusterShard ISolrClusterShards.this[string name] {
            get {
                if (name == null)
                    return Default;
                ISolrClusterShard shard;
                TryGetValue(name, out shard);
                return shard;
            }
        }

        ISolrClusterShard ISolrClusterShards.this[int? hash] {
            get {
                if (!hash.HasValue)
                    return Default;
                foreach (var shard in Values)
                    if (shard.Range.Start <= hash.Value && shard.Range.End >= hash.Value)
                        return shard;
                return null;
            }
        }

        public ISolrClusterCore Core { get; private set; }

        public ISolrClusterShard Default { get; private set; }
    }
}
