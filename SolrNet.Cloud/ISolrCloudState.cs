using System;
using System.Collections.Generic;

namespace SolrNet.Cloud {
    public interface ISolrCloudState : IDisposable {
        IEnumerable<ISolrCloudNode> Nodes { get; }

        void Init();
    }
}