using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    public class SolrCloudShard {
        public SolrCloudShard(bool isActive, string name, int? rangeEnd, int? rangeStart, IDictionary<string, SolrCloudReplica> replicas) {
            if (replicas == null)
                throw new ArgumentNullException("replicas");
            IsActive = isActive;
            Name = name;
            RangeEnd = rangeEnd;
            RangeStart = rangeStart;
            Replicas = replicas;
        }

        public bool IsActive { get; private set; }

        public string Name { get; private set; }

        public int? RangeEnd { get; private set; }

        public int? RangeStart { get; private set; }

        public IDictionary<string, SolrCloudReplica> Replicas { get; set; }
    }
}
