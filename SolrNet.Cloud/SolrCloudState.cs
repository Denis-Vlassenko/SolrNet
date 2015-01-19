using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrCloudState {
        public SolrCloudState(IDictionary<string, SolrCloudCollection> collections) {
            Collections = collections;
        }

        public IDictionary<string, SolrCloudCollection> Collections { get; private set; }
    }
}