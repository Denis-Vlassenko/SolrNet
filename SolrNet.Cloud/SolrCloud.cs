using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SolrNet.Cloud
{
    public class SolrCloud : ISolrCloud {
        public SolrCloud(IOperationsResolver resolver, TimeSpan refreshInterval, params ISolrCloudNode[] nodes){
            this.resolver = resolver;
            this.nodes = nodes;
            this.timer = new Timer(this.Refresh, null, TimeSpan.FromTicks(0), refreshInterval);
        }

        ~SolrCloud() {
            this.Dispose(false);
        }

        private readonly int count;

        private readonly ISolrCloudNode[] nodes;

        private readonly IOperationsResolver resolver;

        private long refreshCursor;

        private long selectCursor;

        private readonly Timer timer;

        public IEnumerable<ISolrCloudNode> Leaders{
            get {
                yield return
                    this.nodes.FirstOrDefault(node => node.IsActive && node.IsLeader)
                    ?? this.nodes.FirstOrDefault(node => node.IsActive)
                    ?? this.nodes.FirstOrDefault();
            }
        }

        public IEnumerable<ISolrCloudNode> Replicas {
            get {
                var skip = (int)(Interlocked.Increment(ref this.selectCursor) % nodes.Length);

            }
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            this.timer.Dispose();
        }

        private void Refresh(object state) {
            throw new NotImplementedException();
        }
    }
}
