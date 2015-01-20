using System;

namespace SolrNet.Cloud
{
    public interface ISolrCloudStateProvider : IDisposable {
        string Key { get; }

        SolrCloudState GetCloudState();

        void Init();
    }
}
