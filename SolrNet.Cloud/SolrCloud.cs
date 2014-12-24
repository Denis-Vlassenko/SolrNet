using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SolrNet.Cloud
{
    public class SolrCloud : ISolrCloud {
        public SolrCloud(TimeSpan updateInterval, Action<ISolrCloudNode> updateMethod, params ISolrCloudNode[] cloudNodes) {
            this.cloudNodes = cloudNodes;
            this.updateTimer = new Timer(this.Update, null, TimeSpan.FromTicks(0), updateInterval);
            this.updateMethod = updateMethod;
        }

        ~SolrCloud() {
            this.Dispose(false);
        }

        private readonly ISolrCloudNode[] cloudNodes;

        private long selectCursor;

        private long updateCursor;

        private readonly Action<ISolrCloudNode> updateMethod;

        private readonly Timer updateTimer;

        public IEnumerable<ISolrCloudNode> Leaders{
            get {
                var leader = this.cloudNodes.FirstOrDefault(node => node.IsActive && node.IsLeader);
                if (null != leader)
                    yield return leader;
                var index = 0;
                while (index++ < cloudNodes.Length)
                {
                    var node = this.cloudNodes[index];
                    if (node.IsActive)
                        yield return node;
                }
                index = 0;
                while (index++ < cloudNodes.Length)
                    yield return this.cloudNodes[index];
            }
        }

        public IEnumerable<ISolrCloudNode> Replicas {
            get {
                var index = Interlocked.Increment(ref this.selectCursor);
                var limit = cloudNodes.Length;
                while (0 < limit--) {
                    var node = this.cloudNodes[index++ % cloudNodes.Length];
                    if (node.IsActive)
                        yield return node;
                }
                limit = cloudNodes.Length;
                while (0 < limit--)
                    yield return this.cloudNodes[index++%cloudNodes.Length];
            }
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            this.updateTimer.Dispose();
        }

        private void Update(object state) {
            var index = Interlocked.Increment(ref this.updateCursor);
            var limit = cloudNodes.Length;
            while (0 < limit--) {
                var node = this.cloudNodes[index++%cloudNodes.Length];
                if (node.IsActive)
                    continue;
                this.updateMethod(node);
                break;
            }
        }
    }
}