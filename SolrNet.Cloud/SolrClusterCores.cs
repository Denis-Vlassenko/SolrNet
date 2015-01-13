using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterCores : Dictionary<string, ISolrClusterCore>, ISolrClusterCores {
        private SolrClusterCores(JObject json) : base(StringComparer.OrdinalIgnoreCase) {
            foreach (var property in json.Properties()) {
                var core = new SolrClusterCore(property);
                base.Add(core.Name, core);
            }
        }

        IEnumerator<ISolrClusterCore> IEnumerable<ISolrClusterCore>.GetEnumerator() {
            return base.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public static ISolrClusterCores ParseJson(string json) {
            return new SolrClusterCores(JObject.Parse(json));
        }
    }
}