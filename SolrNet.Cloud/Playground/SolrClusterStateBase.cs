using System;
using System.Threading;

namespace SolrNet.Cloud.Playground
{
    public abstract class SolrClusterStateBase : ISolrClusterState {
        protected SolrClusterStateBase(StateParser parser) {
            if (null == parser) throw new ArgumentNullException("parser");
            this.parser = parser;
            this.sync = new object();
        }

        private bool disposed;

        private bool initialized;

        private readonly StateParser parser;

        private readonly object sync;

        public delegate ISolrClusterCores StateParser(string state);

        public ISolrClusterCores Cores { get; private set; }

        public void Dispose() {
            lock (sync)
                if (!disposed) {
                    OnDispose();
                    disposed = true;
                }
        }

        protected void Update(string state) {
            Cores = parser(state);
        }

        public bool Initialize() {
            lock (sync)
                if (!initialized)
                    initialized = OnInitialize();
            return initialized;
        }

        protected abstract void OnDispose();

        protected abstract bool OnInitialize();
    }
}
