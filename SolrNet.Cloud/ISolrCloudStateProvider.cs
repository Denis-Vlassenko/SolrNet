using System;

namespace SolrNet.Cloud
{
    public interface ISolrCloudStateProvider : IDisposable {
        SolrCloudState GetCloudState();

        void Init();
    }
}
