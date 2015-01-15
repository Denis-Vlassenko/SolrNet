using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrCloudState : IDisposable {
        IEnumerable<SolrCloudNode> Nodes { get; }

        void Init();
    }
}