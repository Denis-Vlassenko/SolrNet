using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud.Playground
{
    public class SolrClusterShards : Dictionary<string, ISolrClusterShard>, ISolrClusterShards {
        public SolrClusterShards(JObject json) : base(StringComparer.OrdinalIgnoreCase) {
            foreach (var property in json.Properties()) {
                var shard = new SolrClusterShard(property);
                base.Add(shard.Range, shard);
            }
        }

        IEnumerator<ISolrClusterShard> IEnumerable<ISolrClusterShard>.GetEnumerator() {
            return base.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
