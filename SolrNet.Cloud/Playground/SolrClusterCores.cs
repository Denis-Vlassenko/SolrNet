using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud {
    public class SolrClusterCores : Dictionary<string, ISolrClusterCore>, ISolrClusterCores {
        public SolrClusterCores(JObject json) {
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
    }
}