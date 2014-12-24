using System;
using System.Collections.Generic;

namespace SolrNet.Cloud
{
    public interface ISolrCloud : IDisposable{
        IEnumerable<ISolrCloudNode> Leaders { get; }

        IEnumerable<ISolrCloudNode> Replicas { get; }
    }
}