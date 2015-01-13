using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrClusterCores : IEnumerable<ISolrClusterCore> {
        ISolrClusterCore this[string name] { get; }
    }
}
