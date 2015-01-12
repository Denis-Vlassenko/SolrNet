using System.Collections;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrClusterCores : Dictionary<string, ISolrClusterCore>, ISolrClusterCores {
        IEnumerator<ISolrClusterCore> IEnumerable<ISolrClusterCore>.GetEnumerator() {
            return base.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}