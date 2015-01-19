using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrCloudCollection {
        public SolrCloudCollection(string name, SolrCloudRouter router, IDictionary<string, SolrCloudShard> shards) {
            if (router == null)
                throw new ArgumentNullException("router");
            if (shards == null)
                throw new ArgumentNullException("shards");
            Name = name;
            Router = router;
            Shards = shards;
        }

        public string Name { get; private set; }

        public SolrCloudRouter Router { get; set; }

        public IDictionary<string, SolrCloudShard> Shards { get; set; }
    }
}
